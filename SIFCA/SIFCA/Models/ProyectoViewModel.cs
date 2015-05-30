using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIFCA.Models
{
    public class ProyectoViewModel
    {
        public PROYECTO Proyecto { get; set; }
        public List<EspecieViewModel> Especies { get; set; }
        public List<EstratoViewModel> Estratos { get; set; }
        public List<FormulaViewModel> Formulas { get; set; }
        public List<CostoViewModel> Costos { get; set; }
        public List<LocalidadViewModel> Localidades { get; set; }
        public List<TipoLineaInventarioViewModel> TipoLineaInventario { get; set; }

        public ProyectoViewModel()
        {
            Proyecto = new PROYECTO();
            Especies = new List<EspecieViewModel>();
            Estratos = new List<EstratoViewModel>();
            Formulas = new List<FormulaViewModel>();
            Costos = new List<CostoViewModel>();
            Localidades = new List<LocalidadViewModel>();
            TipoLineaInventario = new List<TipoLineaInventarioViewModel>();
        }
    }
}