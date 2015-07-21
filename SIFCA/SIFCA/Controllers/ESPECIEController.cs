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
    public class ESPECIEController : Controller
    {
        private Entities db = new Entities();

        // GET: ESPECIE
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Especie";
            var eSPECIE = db.ESPECIE.Include(e => e.GRUPOCOMERCIAL);
            return View(eSPECIE.ToList());
        }

        // GET: ESPECIE/Details/5
       
        public ActionResult Details(Guid? id)
        {
            try
            {
                ViewBag.MenuActivo = "Especie";
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ESPECIE eSPECIE = db.ESPECIE.Find(id);
                if (eSPECIE == null)
                {
                    return HttpNotFound();
                }
                List<KeyValuePair<string, string>> grupoEcologico = new List<KeyValuePair<string, string>>();
                grupoEcologico.Add(new KeyValuePair<string, string>("NT", "No tolerante"));
                grupoEcologico.Add(new KeyValuePair<string, string>("TL", "Tolerante"));
                ViewBag.GRUPOCOM = new SelectList(db.GRUPOCOMERCIAL, "GRUPOCOM", "DESCRIPGRUPO");
                ViewBag.GRUPOECOLOGICO = new SelectList(grupoEcologico, "Key", "Value");
                return View(eSPECIE);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        // GET: ESPECIE/Create
       
        public ActionResult Create()
        {
            try
            {
                ViewBag.MenuActivo = "Especie";
                List<KeyValuePair<string, string>> grupoEcologico = new List<KeyValuePair<string, string>>();
                grupoEcologico.Add(new KeyValuePair<string, string>("NT", "No tolerante"));
                grupoEcologico.Add(new KeyValuePair<string, string>("TL", "Tolerante"));
                ViewBag.GRUPOCOM = new SelectList(db.GRUPOCOMERCIAL, "GRUPOCOM", "DESCRIPGRUPO");
                ViewBag.GRUPOECOLOGICO = new SelectList(grupoEcologico, "Key", "Value");
                return View();
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        // POST: ESPECIE/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "CODESP,GRUPOCOM,NOMCOMUN,NOMCIENTIFICO,FAMILIA,ZONAGEOGRAFICA,ZONADEVIDA,DIAMMINCORTE,GRUPOECOLOGICO")] ESPECIE eSPECIE)
        {
            try
            {
                ViewBag.MenuActivo = "Especie";
                eSPECIE.CODESP = eSPECIE.CODESP == Guid.Empty ? Guid.NewGuid() : eSPECIE.CODESP;
                if (ModelState.IsValid)
                {
                    db.ESPECIE.Add(eSPECIE);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                List<KeyValuePair<string, string>> grupoEcologico = new List<KeyValuePair<string, string>>();
                grupoEcologico.Add(new KeyValuePair<string, string>("NT", "No tolerante"));
                grupoEcologico.Add(new KeyValuePair<string, string>("TL", "Tolerante"));
                ViewBag.GRUPOCOM = new SelectList(db.GRUPOCOMERCIAL, "GRUPOCOM", "DESCRIPGRUPO");
                ViewBag.GRUPOECOLOGICO = new SelectList(grupoEcologico, "Key", "Value");
                return View(eSPECIE);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        // GET: ESPECIE/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            try
            {
                ViewBag.MenuActivo = "Especie";
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ESPECIE eSPECIE = db.ESPECIE.Find(id);
                if (eSPECIE == null)
                {
                    return HttpNotFound();
                }
                List<KeyValuePair<string, string>> grupoEcologico = new List<KeyValuePair<string, string>>();
                grupoEcologico.Add(new KeyValuePair<string, string>("NT", "No tolerante"));
                grupoEcologico.Add(new KeyValuePair<string, string>("TL", "Tolerante"));
                ViewBag.GRUPOCOM = new SelectList(db.GRUPOCOMERCIAL, "GRUPOCOM", "DESCRIPGRUPO");
                ViewBag.GRUPOECOLOGICO = new SelectList(grupoEcologico, "Key", "Value");
                return View(eSPECIE);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        // POST: ESPECIE/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "CODESP,GRUPOCOM,NOMCOMUN,NOMCIENTIFICO,FAMILIA,ZONAGEOGRAFICA,ZONADEVIDA,DIAMMINCORTE,,GRUPOECOLOGICO")] ESPECIE eSPECIE)
        {
            try
            {
                ViewBag.MenuActivo = "Especie";
                if (ModelState.IsValid)
                {
                    db.Entry(eSPECIE).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                List<KeyValuePair<string, string>> grupoEcologico = new List<KeyValuePair<string, string>>();
                grupoEcologico.Add(new KeyValuePair<string, string>("NT", "No tolerante"));
                grupoEcologico.Add(new KeyValuePair<string, string>("TL", "Tolerante"));
                ViewBag.GRUPOCOM = new SelectList(db.GRUPOCOMERCIAL, "GRUPOCOM", "DESCRIPGRUPO");
                ViewBag.GRUPOECOLOGICO = new SelectList(grupoEcologico, "Key", "Value");
                return View(eSPECIE);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        // POST: ESPECIE/Delete/5
        [HttpPost]
       
        public ActionResult Delete(Guid? id)
        {
            try
            {
                ViewBag.MenuActivo = "Especie";
                ESPECIE eSPECIE = db.ESPECIE.Find(id);
                db.ESPECIE.Remove(eSPECIE);
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
