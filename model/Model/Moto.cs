using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Moto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Marca { get; set; }
        public required string Modelo { get; set; }
        public string? Placa { get; set; }
        public string? Chassi { get; set; }
        public Guid IdStatusOperacional { get; set; }
        public Guid IdRastreador { get; set; }
        public DateTime DtCadastro { get; set; }
        public DateTime DtAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}
