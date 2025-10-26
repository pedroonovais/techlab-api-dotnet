namespace api.Configuration
{
    /// <summary>
    /// Configurações para autenticação JWT
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Chave secreta usada para assinar os tokens
        /// </summary>
        public required string SecretKey { get; set; }

        /// <summary>
        /// Emissor do token (quem gerou o token)
        /// </summary>
        public required string Issuer { get; set; }

        /// <summary>
        /// Audiência do token (para quem o token é destinado)
        /// </summary>
        public required string Audience { get; set; }

        /// <summary>
        /// Tempo de expiração do token em minutos
        /// </summary>
        public int ExpirationInMinutes { get; set; }
    }
}

