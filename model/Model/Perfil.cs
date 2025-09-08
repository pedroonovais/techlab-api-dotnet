using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class Perfil
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required short NivelAcesso { get; set; }
    }
}
