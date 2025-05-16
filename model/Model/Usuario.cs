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
        private long id { get; set; }
        
        [Required]
        private string nome { get; set; }

        [Required]
        private string email { get; set; }

        [Required]
        private string senha { get; set; }

        [Required]
        private int cpf { get; set; }

        [Required]
        private string status { get; set; }

        [Required]
        private string perfil { get; set; }

        [Required]
        private string area { get; set; }

        [Required]
        private DateTime dtCriacao { get; set; }

        [Required]
        private DateTime dtAlteracao { get; set; }
    }
}
