using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIFCA.Models
{
    public class EspecieViewModel
    {
        public Guid codEspecie { get; set; }
        public string Familia { get; set; }
        public string NombreCientifico { get; set; }
        public string NombreComun { get; set; }
        public bool Seleccionar { get; set; }
    }
}