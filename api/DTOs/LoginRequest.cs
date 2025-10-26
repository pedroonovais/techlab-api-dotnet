using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO para requisição de login
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// E-mail do usuário
        /// </summary>
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public required string Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public required string Senha { get; set; }
    }
}

