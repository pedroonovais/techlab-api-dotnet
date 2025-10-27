using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TechLab.IntegrationTests.Helpers
{
    /// <summary>
    /// Handler de autenticação para testes que sempre autentica o usuário
    /// Elimina a necessidade de tokens JWT reais durante os testes de integração
    /// </summary>
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestScheme";

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        /// <summary>
        /// Autentica automaticamente todas as requisições em ambiente de teste
        /// Cria um usuário fictício com claims básicas para simular autenticação real
        /// </summary>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Criar claims de teste para simular usuário autenticado
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Email, "test@techlab.com"),
                new Claim("PerfilId", Guid.NewGuid().ToString())
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            // Log para facilitar debug dos testes
            Logger.LogInformation("[TestAuth] Autenticando usuário de teste: {UserName}", "TestUser");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

