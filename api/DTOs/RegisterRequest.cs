using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO para requisição de registro de novo usuário
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MinLength(2, ErrorMessage = "O nome deve ter no mínimo 2 caracteres")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public required string Nome { get; set; }

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
        [MaxLength(100, ErrorMessage = "A senha deve ter no máximo 100 caracteres")]
        public required string Senha { get; set; }

        /// <summary>
        /// Confirmação da senha
        /// </summary>
        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação não coincidem")]
        public required string ConfirmacaoSenha { get; set; }

        /// <summary>
        /// ID do perfil do usuário (opcional - se não fornecido, será atribuído um perfil padrão)
        /// </summary>
        public Guid? PerfilId { get; set; }
    }
}

