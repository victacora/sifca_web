using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIFCA.Models
{
    public class FormulaViewModel
    {
        public Guid codFormula { get; set; }
        public string Nombre { get; set; }
        public bool Seleccionar { get; set; }
    }
}