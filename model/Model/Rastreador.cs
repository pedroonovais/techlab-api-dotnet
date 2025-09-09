using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Rastreador
    {
        public Guid Id { get; set; }
        private string? NumeroSerie { get; set; }
        private string? Modelo { get; set; }
        private DateTime DtCadastro { get; set; }
        private DateTime DtAtualizacao { get; set; }
        private bool Ativo { get; set; }
    }
}
