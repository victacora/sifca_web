using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIFCA.Models
{
    public class TipoLineaInventarioViewModel
    {
        public Guid codTipoLineaInventario { get; set; }
        public string Nombre { get; set; }
        public bool Seleccionar { get; set; }
    }
}