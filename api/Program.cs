using data.Context;
using Microsoft.EntityFrameworkCore;
using service.Service;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<MotoService>();
builder.Services.AddScoped<PatioService>();
builder.Services.AddScoped<PerfilService>();
builder.Services.AddScoped<RastreadorService>();
builder.Services.AddScoped<StatusOperacionalService>();
builder.Services.AddScoped<UsuarioService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuração de versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
}).AddMvc();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("data")));

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

// Health Checks UI
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(30); // Avalia a cada 30 segundos
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.AddHealthCheckEndpoint("TechLab API", "/health");
}).AddInMemoryStorage();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechLab API",
        Version = "v1",
        Description = "API do sistema de gerenciamento de P�tios da TechLab",
        Contact = new OpenApiContact
        {
            Name = "Pedro Novais",
            Url = new Uri("https://www.linkedin.com/in/pedroonovais/")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // cria/aplica migrations automaticamente
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

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia o endpoint de Health Checks com detalhes em JSON
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
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
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Endpoint para readiness (verifica dependências como DB)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// UI visual para Health Checks
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.MapControllers();

app.Run();
