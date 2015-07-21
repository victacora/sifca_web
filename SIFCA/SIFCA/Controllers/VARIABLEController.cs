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
    public class VARIABLEController : Controller
    {
        private Entities db = new Entities();

        // GET: VARIABLE
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Variable";
            return View(db.VARIABLE.ToList());
        }

        // GET: VARIABLE/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "Variable";
            List<KeyValuePair<string, string>> tipoDominio = new List<KeyValuePair<string, string>>();
            tipoDominio.Add(new KeyValuePair<string, string>("N", "No Aplica"));
            tipoDominio.Add(new KeyValuePair<string, string>("U", "Unica seleccion"));
            tipoDominio.Add(new KeyValuePair<string, string>("M", "Seleccion multiple"));
            ViewBag.TIPODOMINIO = new SelectList(tipoDominio, "Key", "Value");
            List<KeyValuePair<string, string>> siNo = new List<KeyValuePair<string, string>>();
            siNo.Add(new KeyValuePair<string, string>("S", "Si"));
            siNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.VARIABLEDESCRIPTIVA = new SelectList(siNo, "Key", "Value");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VARIABLE vARIABLE = db.VARIABLE.Find(id);
            if (vARIABLE == null)
            {
                return HttpNotFound();
            }
            return View(vARIABLE);
        }

        // GET: VARIABLE/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "Variable";
            List<KeyValuePair<string, string>> tipoDominio = new List<KeyValuePair<string, string>>();
            tipoDominio.Add(new KeyValuePair<string, string>("N", "No Aplica"));
            tipoDominio.Add(new KeyValuePair<string, string>("U", "Unica seleccion"));
            tipoDominio.Add(new KeyValuePair<string, string>("M", "Seleccion multiple"));
            ViewBag.TIPODOMINIO = new SelectList(tipoDominio, "Key", "Value");
            List<KeyValuePair<string, string>> siNo = new List<KeyValuePair<string, string>>();
            siNo.Add(new KeyValuePair<string, string>("S", "Si"));
            siNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.VARIABLEDESCRIPTIVA = new SelectList(siNo, "Key", "Value");
            return View();
        }

        // POST: VARIABLE/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NROVARIABLE,ABREVIATURA,NOMBRE,DESCRIPCION,VARIABLEDESCRIPTIVA,TIPODOMINIO")] VARIABLE vARIABLE)
        {
            ViewBag.MenuActivo = "Variable";
            List<KeyValuePair<string, string>> tipoDominio = new List<KeyValuePair<string, string>>();
            tipoDominio.Add(new KeyValuePair<string, string>("N", "No Aplica"));
            tipoDominio.Add(new KeyValuePair<string, string>("U", "Unica seleccion"));
            tipoDominio.Add(new KeyValuePair<string, string>("M", "Seleccion multiple"));
            ViewBag.TIPODOMINIO = new SelectList(tipoDominio, "Key", "Value");
            List<KeyValuePair<string, string>> siNo = new List<KeyValuePair<string, string>>();
            siNo.Add(new KeyValuePair<string, string>("S", "Si"));
            siNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.VARIABLEDESCRIPTIVA = new SelectList(siNo, "Key", "Value");
            if (ModelState.IsValid)
            {
                vARIABLE.NROVARIABLE = Guid.NewGuid();
                db.VARIABLE.Add(vARIABLE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vARIABLE);
        }

        // GET: VARIABLE/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Variable";

            List<KeyValuePair<string, string>> tipoDominio = new List<KeyValuePair<string, string>>();
            tipoDominio.Add(new KeyValuePair<string, string>("N", "No Aplica"));
            tipoDominio.Add(new KeyValuePair<string, string>("U", "Unica seleccion"));
            tipoDominio.Add(new KeyValuePair<string, string>("M", "Seleccion multiple"));
            ViewBag.TIPODOMINIO = new SelectList(tipoDominio, "Key", "Value");
            List<KeyValuePair<string, string>> siNo = new List<KeyValuePair<string, string>>();
            siNo.Add(new KeyValuePair<string, string>("S", "Si"));
            siNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.VARIABLEDESCRIPTIVA = new SelectList(siNo, "Key", "Value");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VARIABLE vARIABLE = db.VARIABLE.Find(id);
            if (vARIABLE == null)
            {
                return HttpNotFound();
            }
            return View(vARIABLE);
        }

        // POST: VARIABLE/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NROVARIABLE,ABREVIATURA,NOMBRE,DESCRIPCION,VARIABLEDESCRIPTIVA,TIPODOMINIO")] VARIABLE vARIABLE)
        {
            ViewBag.MenuActivo = "Variable";
            List<KeyValuePair<string, string>> tipoDominio = new List<KeyValuePair<string, string>>();
            tipoDominio.Add(new KeyValuePair<string, string>("N", "No Aplica"));
            tipoDominio.Add(new KeyValuePair<string, string>("U", "Unica seleccion"));
            tipoDominio.Add(new KeyValuePair<string, string>("M", "Seleccion multiple"));
            ViewBag.TIPODOMINIO = new SelectList(tipoDominio, "Key", "Value");
            List<KeyValuePair<string, string>> siNo = new List<KeyValuePair<string, string>>();
            siNo.Add(new KeyValuePair<string, string>("S", "Si"));
            siNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.VARIABLEDESCRIPTIVA = new SelectList(siNo, "Key", "Value");
            if (ModelState.IsValid)
            {
                db.Entry(vARIABLE).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vARIABLE);
        }

        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "Variable";
                VARIABLE vARIABLE = db.VARIABLE.Find(id);
                db.VARIABLE.Remove(vARIABLE);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch (Exception ex)
            {//TODO: Log error				
                return Content("<strong class='text-danger'>Resumen del error:</strong><br />" + ex.Message + "<br /><strong class='text-danger'>Detalles del error:</strong><br />" + ex.InnerException);
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
