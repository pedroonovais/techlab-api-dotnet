using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Usuario
    {
        public Guid id { get; set; }

        [Required]
        public required string nome { get; set; }

        [Required]
        public required string email { get; set; }

        [Required]
        public required string senha { get; set; }

        [Required]
        public string cpf { get; set; }

        [Required]
        public required string status { get; set; }

        [Required]
        public required string perfil { get; set; }

        [Required]
        public required string area { get; set; }

        [Required]
        public DateTime dtCriacao { get; set; }

        [Required]
        public DateTime dtAlteracao { get; set; }
    }
}
