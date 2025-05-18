using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Moto
    {
        public Guid id { get; set; }
        public required string marca { get; set; }
        public required string modelo { get; set; }
        private string? placa { get; set; }
        private string? chassi { get; set; }
        private string? motor { get; set; }
        private string? imeiIot { get; set; }
        private long rfId { get; set; }
        private DateTime dataCadastro { get; set; }
        private DateTime dataAtualizacao { get; set; }
    }
}
