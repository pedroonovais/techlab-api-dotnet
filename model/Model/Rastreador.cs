using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Rastreador
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? NumeroSerie { get; set; }
        public string? Modelo { get; set; }
        public DateTime DtCadastro { get; set; }
        public DateTime DtAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}
