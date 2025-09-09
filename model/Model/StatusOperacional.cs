using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Model
{
    public class StatusOperacional
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Descricao { get; set; }
    }
}
