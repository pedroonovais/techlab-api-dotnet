using System;
using System.Collections.Generic;
using System.Linq;namespace library.Model
{
    public class Patio
    {
        public Guid id { get; set; }
        public required string nome { get; set; }
        public required string localizacao { get; set; }

        public int capacidadeTotal { get; set; }
        public int vagasDisponiveis { get; set; }

        public required string descricao { get; set; }
        public bool ativo { get; set; }

        public ICollection<Moto> ?motos { get; set; }

        public DateTime dataCadastro { get; set; }
        public DateTime dataAtualizacao { get; set; }
    }
}
