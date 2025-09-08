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
        private Guid IdStatusOperacional { get; set; }

        [Required]
        private string Descricao { get; set; }
    }
}
