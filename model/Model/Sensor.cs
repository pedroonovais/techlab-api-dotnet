using System;
using System.ComponentModel.DataAnnotations;

namespace library.Model
{
    public class Sensor
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public required string codigoIdentificacao { get; set; } 

        [Required]
        public required string tipo { get; set; }

        [Required]
        public required string localizacaoFisica { get; set; } 

        [Required]
        public required string status { get; set; } 

        public DateTime dtInstalacao { get; set; }
        public DateTime dtUltimaLeitura { get; set; }
    }
}
