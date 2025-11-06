using data.Context;
using Microsoft.EntityFrameworkCore;
using service.Service;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<MotoService>();
builder.Services.AddScoped<PatioService>();
builder.Services.AddScoped<PerfilService>();
builder.Services.AddScoped<RastreadorService>();
builder.Services.AddScoped<StatusOperacionalService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();

// Serviço de Machine Learning (Singleton para manter modelo em memória)
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<service.ML.MLService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuração de versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
}).AddMvc();

// Registrar banco de dados apenas se não estiver em ambiente de teste
// Em ambiente de teste, o WebApplicationFactory configurará InMemory
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("data")));
}

// Configuração JWT (não registrar em ambiente de teste)
// O ambiente de teste usará TestAuthenticationHandler configurado no WebApplicationFactory
if (!builder.Environment.IsEnvironment("Testing"))
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey não configurada");

    builder.Services.Configure<JwtSettings>(jwtSettings);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero // Remove o atraso padrão de 5 minutos na expiração
        };

        // Eventos para logging (útil para debug)
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT] Falha na autenticação: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"[JWT] Token validado com sucesso para: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });
}
else
{
    // Em ambiente de teste, apenas adicionar estrutura básica de autenticação
    // O TestAuthenticationHandler será configurado no WebApplicationFactory
    builder.Services.AddAuthentication();
}

builder.Services.AddAuthorization();

// Configuração de Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "PostgreSQL Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "postgresql", "ready" })
    .AddCheck("API Health", () => 
        HealthCheckResult.Healthy("API está respondendo"), 
        tags: new[] { "api", "live" });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechLab API",
        Version = "v1",
        Description = "API do sistema de gerenciamento de Pátios da TechLab",
        Contact = new OpenApiContact
        {
            Name = "Pedro Novais",
            Url = new Uri("https://www.linkedin.com/in/pedroonovais/")
        }
    });

    // Configuração de autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Inicialização que não deve executar em ambiente de testes
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate(); // cria/aplica migrations automaticamente
        
        // Popula o banco de dados com dados iniciais (seed)
        Console.WriteLine("[Startup] Verificando necessidade de seed de dados...");
        await data.Seed.DataSeeder.SeedAsync(db);
        
        // Treinar modelo de ML automaticamente no startup (se o serviço estiver registrado)
        var mlService = scope.ServiceProvider.GetService<service.ML.MLService>();
        if (mlService != null)
        {
            Console.WriteLine("[Startup] Iniciando treinamento do modelo de ML...");
            await mlService.TrainModelAsync();
            Console.WriteLine("[Startup] Modelo de ML treinado e pronto para uso!");
        }
    }
}
else
{
    Console.WriteLine("[Startup] Ambiente de teste detectado - pulando migrations, seed e treinamento ML");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechLab API v1");
    c.RoutePrefix = string.Empty;
});

// HTTPS Redirection apenas em produção (evita warning em desenvolvimento sem HTTPS)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// A ordem é importante: Authentication deve vir antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

// ResponseWriter customizado para Health Checks (sem dependência do HealthChecks.UI)
static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    
    var result = JsonSerializer.Serialize(new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.TotalMilliseconds,
        entries = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description,
            duration = e.Value.Duration.TotalMilliseconds,
            tags = e.Value.Tags,
            data = e.Value.Data,
            exception = e.Value.Exception?.Message
        })
    }, new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });
    
    return context.Response.WriteAsync(result);
}

// Mapeia o endpoint de Health Checks com detalhes em JSON
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthCheckResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

// Endpoint simplificado para liveness (apenas verifica se API está UP)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = WriteHealthCheckResponse
});

// Endpoint para readiness (verifica dependências como DB)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthCheckResponse
});

app.MapControllers();

app.Run();

/// <summary>
/// Tipo parcial público necessário para testes de integração com WebApplicationFactory
/// Permite que o projeto de testes referencie o entrypoint da aplicação quando usa top-level statements
/// </summary>
public partial class Program { }