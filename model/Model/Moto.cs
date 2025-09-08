using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Moto
    {
        public Guid Id { get; set; }
        public required string Marca { get; set; }
        public required string Modelo { get; set; }
        private string? Placa { get; set; }
        private string? Chassi { get; set; }
        private Guid IdStatusOperacional { get; set; }
        private DateTime DtCadastro { get; set; }
        private DateTime DtAtualizacao { get; set; }
        private bool Ativo { get; set; }
    }
}
