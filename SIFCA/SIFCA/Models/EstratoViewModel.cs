using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIFCA.Models
{
    public class EstratoViewModel
    {
        public Int32 codEstrato { get; set; }
        public string Nombre { get; set; }
        public double Peso { get; set; }
        public double TamanioMuestra { get; set; }
        public bool Seleccionar { get; set; }
    }
}