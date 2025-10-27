using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using data.Context;
using Microsoft.AspNetCore.Authentication;
using api.Configuration;
using Microsoft.Extensions.Configuration;

namespace TechLab.IntegrationTests.Helpers
{
    /// <summary>
    /// Factory customizada para testes de integração
    /// Configura ambiente "Testing" com banco InMemory e autenticação falsa
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Configurar ambiente como "Testing"
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Configurar JWT Settings para ambiente de teste
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:SecretKey"] = "test-secret-key-for-integration-tests-minimum-32-characters-long",
                    ["JwtSettings:Issuer"] = "TechLabTestIssuer",
                    ["JwtSettings:Audience"] = "TechLabTestAudience",
                    ["JwtSettings:ExpirationInMinutes"] = "480"
                });
            });

            builder.ConfigureTestServices(services =>
            {
                // Remover todas as instâncias de configuração do PostgreSQL
                var descriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)).ToList();
                
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Adicionar banco InMemory para testes
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryTestDb_{Guid.NewGuid()}");
                });

                // Remover o serviço de ML para evitar treinamento em testes
                services.RemoveAll(typeof(service.ML.MLService));
                
                // Adicionar esquema de autenticação de teste
                services.AddAuthentication(TestAuthenticationHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.SchemeName,
                        options => { });

                // Log para debug: indicar que estamos usando configuração de teste
                Console.WriteLine("[CustomWebApplicationFactory] Configuração de teste aplicada:");
                Console.WriteLine("  - Banco de dados: InMemory");
                Console.WriteLine("  - Autenticação: TestAuthenticationHandler");
                Console.WriteLine("  - Ambiente: Testing");
            });
        }

        /// <summary>
        /// Cria um HttpClient configurado com autenticação de teste
        /// </summary>
        public HttpClient CreateAuthenticatedClient()
        {
            var client = CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // O TestAuthenticationHandler já autentica automaticamente
            // Não é necessário adicionar header Authorization
            Console.WriteLine("[CustomWebApplicationFactory] Cliente HTTP autenticado criado");

            return client;
        }
    }
}

