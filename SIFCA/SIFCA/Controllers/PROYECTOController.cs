using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIFCA.Models;

namespace SIFCA.Controllers
{
    public class PROYECTOController : Controller
    {
        private Entities db = new Entities();

        // GET: PROYECTO
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Proyecto";
            var pROYECTO = db.PROYECTO.Include(p => p.OBJETIVOINVENTARIO).Include(p => p.USUARIO);
            return View(pROYECTO.ToList());
        }

        // GET: PROYECTO/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "Proyecto";

            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectoViewModel pVW = new ProyectoViewModel();
            PROYECTO proyecto = db.PROYECTO.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            else
            {              
                pVW.Proyecto = proyecto;
                pVW.Especies = proyecto.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP, Familia = e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = false }).ToList();
                pVW.Estratos = proyecto.LISTADODEESTRATOS.Select(e => new EstratoViewModel() { codEstrato = e.CODEST, Nombre = e.ESTRATO.DESCRIPESTRATO, TamanioMuestra = 0, Peso = 0, Seleccionar = false }).ToList();
                pVW.Formulas = proyecto.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
                pVW.Localidades = proyecto.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
                pVW.TipoLineaInventario = proyecto.TIPOLINEAINVENTARIO.Select(e => new TipoLineaInventarioViewModel() { codTipoLineaInventario = e.NROTIPOLINEAINV, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
                pVW.Costos = proyecto.LISTADODECOSTOS.Select(e => new CostoViewModel() { codCosto = e.NROCOSTO, Nombre = e.COSTO.NOMBRE, Valor = 0, Seleccionar = false }).ToList();
            }
            return View(pVW);
    }

    // GET: PROYECTO/Create
       
    public ActionResult Create()
    {
        ViewBag.MenuActivo = "Proyecto";

        ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
        List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
        seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
        seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
        ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

        List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
        tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
        tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
        tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
        tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
        tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
        ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

        List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
        tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
        tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
        tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
        ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

        List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
        tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
        tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
        tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
        ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

        ProyectoViewModel pVW = new ProyectoViewModel();
        pVW.Proyecto.FECHA = DateTime.Now;
        pVW.Especies = db.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP,Familia=e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = false }).ToList();
        pVW.Estratos = db.ESTRATO.Select(e => new EstratoViewModel() { codEstrato = e.CODEST, Nombre = e.DESCRIPESTRATO,TamanioMuestra=0,Peso=0, Seleccionar = false }).ToList();
        pVW.Formulas = db.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
        pVW.Localidades = db.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
        pVW.TipoLineaInventario = db.TIPOLINEAINVENTARIO.Select(e => new TipoLineaInventarioViewModel() { codTipoLineaInventario = e.NROTIPOLINEAINV, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
        pVW.Costos = db.COSTO.Select(e => new CostoViewModel() { codCosto = e.NROCOSTO, Nombre = e.NOMBRE,Valor=0, Seleccionar = false }).ToList();
        return View(pVW);
        }

        // POST: PROYECTO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProyectoViewModel pVW)
        {
            ViewBag.MenuActivo = "Proyecto";
            pVW.Proyecto.NROPROY = Guid.NewGuid();
            if (Session["USUARIO"] == null)
            {
                return RedirectToAction("Login", "Account", null);
            }
            else pVW.Proyecto.NROUSUARIO = ((USUARIO)Session["USUARIO"]).NROUSUARIO;
            if (ModelState.IsValid)
            {
                pVW.Proyecto.LISTADODECOSTOS = pVW.Costos.Where(c => c.Seleccionar).Select(c => new LISTADODECOSTOS() { NROCOSTO = c.codCosto, VALOR = c.Valor }).ToList<LISTADODECOSTOS>();
                pVW.Proyecto.LISTADODEESTRATOS = pVW.Estratos.Where(e => e.Seleccionar).Select(e => new LISTADODEESTRATOS() { CODEST = e.codEstrato, NROPROY = pVW.Proyecto.NROPROY, PESO = e.Peso }).ToList<LISTADODEESTRATOS>();

                List<Guid> tipoLineaInvSeleccionadas = pVW.TipoLineaInventario.Where(tVW => tVW.Seleccionar).Select(tVW => tVW.codTipoLineaInventario).ToList<Guid>();
                pVW.Proyecto.TIPOLINEAINVENTARIO = db.TIPOLINEAINVENTARIO.Where(t => tipoLineaInvSeleccionadas.Contains(t.NROTIPOLINEAINV)).ToList<TIPOLINEAINVENTARIO>();

                List<Guid> especiesSeleccionadas = pVW.Especies.Where(eVW => eVW.Seleccionar).Select(eVW => eVW.codEspecie).ToList<Guid>();
                pVW.Proyecto.ESPECIE = db.ESPECIE.Where(e => especiesSeleccionadas.Contains(e.CODESP)).ToList<ESPECIE>();

                List<int> localidadesSeleccionadas = pVW.Localidades.Where(lVW => lVW.Seleccionar).Select(lVW => lVW.codLocalidad).ToList<int>();
                pVW.Proyecto.LOCALIDAD = db.LOCALIDAD.Where(l => localidadesSeleccionadas.Contains(l.CODLOCALIDAD)).ToList<LOCALIDAD>();

                db.PROYECTO.Add(pVW.Proyecto);
                db.SaveChanges();
            
                return RedirectToAction("Index");
            }

            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
 
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            return View(pVW);
        }

        // GET: PROYECTO/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Proyecto";

            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectoViewModel pVW = new ProyectoViewModel();
            PROYECTO proyecto = db.PROYECTO.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            else
            {
                pVW.Proyecto = proyecto;
                List<Guid> especiesSeleccionadas = proyecto.ESPECIE.Select(e => e.CODESP).ToList<Guid>();
                pVW.Especies = db.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP, Familia = e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = (especiesSeleccionadas.Contains(e.CODESP)?true:false) }).ToList();
                List<int> estratosSeleccionados = proyecto.LISTADODEESTRATOS.Select(e => e.CODEST).ToList<int>();
                pVW.Estratos = db.ESTRATO.Select(e => new EstratoViewModel() { codEstrato = e.CODEST, Nombre = e.DESCRIPESTRATO, TamanioMuestra = 0, Peso = 0, Seleccionar = (estratosSeleccionados.Contains(e.CODEST) ? true : false) }).ToList();
                List<Guid> formulasSeleccionadas = proyecto.FORMULA.Select(e => e.NROFORMULA).ToList<Guid>();
                pVW.Formulas = db.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar=(formulasSeleccionadas.Contains(e.NROFORMULA) ? true : false) }).ToList();
                List<int> localidadesSeleccionadas = proyecto.LOCALIDAD.Select(e => e.CODLOCALIDAD).ToList<int>();
                pVW.Localidades = db.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = (localidadesSeleccionadas.Contains(e.CODLOCALIDAD) ? true : false) }).ToList();
                List<Guid> tipoLineasSeleccionadas = proyecto.TIPOLINEAINVENTARIO.Select(e => e.NROTIPOLINEAINV).ToList<Guid>();
                pVW.TipoLineaInventario = db.TIPOLINEAINVENTARIO.Select(e => new TipoLineaInventarioViewModel() { codTipoLineaInventario = e.NROTIPOLINEAINV, Nombre = e.NOMBRE, Seleccionar = (tipoLineasSeleccionadas.Contains(e.NROTIPOLINEAINV) ? true : false) }).ToList();
                List<Guid> costosSeleccionadas = proyecto.LISTADODECOSTOS.Select(e => e.NROCOSTO).ToList<Guid>();
                pVW.Costos = db.COSTO.Select(e => new CostoViewModel() { codCosto = e.NROCOSTO, Nombre = e.NOMBRE, Valor = 0, Seleccionar = (costosSeleccionadas.Contains(e.NROCOSTO) ? true : false) }).ToList();
            }
            return View(pVW);
        }

        // POST: PROYECTO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(ProyectoViewModel pVW)
        {
            ViewBag.MenuActivo = "Proyecto";      
            PROYECTO proyecto = db.PROYECTO.Find(pVW.Proyecto.NROPROY);
            if (ModelState.IsValid && proyecto != null)
            {

                proyecto.ESPECIE.Clear();
                proyecto.LISTADODEESTRATOS.Clear();
                proyecto.FORMULA.Clear();
                proyecto.LOCALIDAD.Clear();
                proyecto.TIPOLINEAINVENTARIO.Clear();
                proyecto.LISTADODECOSTOS.Clear();

                pVW.Proyecto.LISTADODECOSTOS = pVW.Costos.Where(c => c.Seleccionar).Select(c => new LISTADODECOSTOS() { NROCOSTO = c.codCosto, VALOR = c.Valor }).ToList<LISTADODECOSTOS>();
                pVW.Proyecto.LISTADODEESTRATOS = pVW.Estratos.Where(e => e.Seleccionar).Select(e => new LISTADODEESTRATOS() { CODEST = e.codEstrato, NROPROY = pVW.Proyecto.NROPROY, PESO = e.Peso }).ToList<LISTADODEESTRATOS>();

                List<Guid> tipoLineaInvSeleccionadas = pVW.TipoLineaInventario.Where(tVW => tVW.Seleccionar).Select(tVW => tVW.codTipoLineaInventario).ToList<Guid>();
                pVW.Proyecto.TIPOLINEAINVENTARIO = db.TIPOLINEAINVENTARIO.Where(t => tipoLineaInvSeleccionadas.Contains(t.NROTIPOLINEAINV)).ToList<TIPOLINEAINVENTARIO>();

                List<Guid> especiesSeleccionadas = pVW.Especies.Where(eVW => eVW.Seleccionar).Select(eVW => eVW.codEspecie).ToList<Guid>();
                pVW.Proyecto.ESPECIE = db.ESPECIE.Where(e => especiesSeleccionadas.Contains(e.CODESP)).ToList<ESPECIE>();

                List<int> localidadesSeleccionadas = pVW.Localidades.Where(lVW => lVW.Seleccionar).Select(lVW => lVW.codLocalidad).ToList<int>();
                pVW.Proyecto.LOCALIDAD = db.LOCALIDAD.Where(l => localidadesSeleccionadas.Contains(l.CODLOCALIDAD)).ToList<LOCALIDAD>();
                
                db.Entry(pVW.Proyecto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "tipoInventario", "DESCRIPOBJETINV", pVW.Proyecto.NOMBRETIPOINV);
 
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            return View(pVW);
        }

        // POST: PROYECTO/Delete/5
        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "Proyecto";
                PROYECTO pROYECTO = db.PROYECTO.Find(id);
                db.PROYECTO.Remove(pROYECTO);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
