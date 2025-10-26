namespace api.DTOs
{
    /// <summary>
    /// DTO para resposta de login bem-sucedido
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT gerado
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// Tipo do token (geralmente "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Tempo de expiração do token em segundos
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// ID do usuário autenticado
        /// </summary>
        public Guid UsuarioId { get; set; }

        /// <summary>
        /// Nome do usuário autenticado
        /// </summary>
        public required string Nome { get; set; }

        /// <summary>
        /// E-mail do usuário autenticado
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// ID do perfil do usuário
        /// </summary>
        public Guid PerfilId { get; set; }
    }
}

