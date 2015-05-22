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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROYECTO pROYECTO = db.PROYECTO.Find(id);
            if (pROYECTO == null)
            {
                return HttpNotFound();
            }
            return View(pROYECTO);
        }

        // GET: PROYECTO/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "Proyecto";

            ViewBag.Proyecto_NOMBRETIPOINV = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
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
 
            ProyectoViewModel pVW = new ProyectoViewModel();
            pVW.Proyecto.FECHA = DateTime.Now;
            pVW.Especies = db.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP,Familia=e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = false }).ToList();
            pVW.Estratos = db.ESTRATO.Select(e => new EstratoViewModel() { codEstrato = e.CODEST, Nombre = e.DESCRIPESTRATO,TamanioMuestra=0,Peso=0, Seleccionar = false }).ToList();
            pVW.Formulas = db.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
            pVW.Localidades = db.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
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
            pVW.Proyecto.USUARIO = (USUARIO)Session["USUARIO"];
           /* if (ModelState.IsValid)
            {
                db.PROYECTO.Add(proyecto.Proyecto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }*/
            
            ViewBag.Proyecto_NOMBRETIPOINV = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
 
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
 
            return View(pVW);
        }

        // GET: PROYECTO/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Proyecto";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROYECTO pROYECTO = db.PROYECTO.Find(id);
            if (pROYECTO == null)
            {
                return HttpNotFound();
            }

            ViewBag.Proyecto_NOMBRETIPOINV = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
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
 
            return View(pROYECTO);
        }

        // POST: PROYECTO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(ProyectoViewModel proyecto)
        {
            ViewBag.MenuActivo = "Proyecto";
            proyecto.Proyecto.USUARIO = (USUARIO)Session["USUARIO"];        
            if (ModelState.IsValid)
            {
                db.Entry(proyecto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Proyecto_NOMBRETIPOINV = new SelectList(db.OBJETIVOINVENTARIO, "Proyecto_NOMBRETIPOINV", "DESCRIPOBJETINV", proyecto.Proyecto.NOMBRETIPOINV);
 
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
 
            return View(proyecto);
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
