using System.ComponentModel.DataAnnotations;

namespace api.DTOs.ML
{
    /// <summary>
    /// DTO para requisição de previsão de manutenção
    /// </summary>
    public class PreverManutencaoRequest
    {
        /// <summary>
        /// ID da moto para análise de manutenção
        /// </summary>
        [Required(ErrorMessage = "O ID da moto é obrigatório")]
        public Guid MotoId { get; set; }
    }
}

