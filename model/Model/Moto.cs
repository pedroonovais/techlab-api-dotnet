using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Moto
    {
        public int id { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        private string placa { get; set; }
        private string chassi { get; set; }
        private string motor { get; set; }
        private string imeiIot { get; set; }
        private long rfId { get; set; }
        private DateTime dataCadastro { get; set; }
        private DateTime dataAtualizacao { get; set; }
    }
}
