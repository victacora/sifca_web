using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIFCA.Models
{
    public class CostoViewModel
    {
        public Guid codCosto { get; set; }
        public string Nombre { get; set; }
        public double Valor { get; set; }
        public bool Seleccionar { get; set; }
    }
}