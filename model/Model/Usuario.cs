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
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Senha { get; set; }

        [Required]
        public required Guid Perfil { get; set; }

        [Required]
        public DateTime DtCriacao { get; set; }

        [Required]
        public DateTime DtAlteracao { get; set; }

        [Required]
        public required bool Ativo { get; set; }

    }
}
