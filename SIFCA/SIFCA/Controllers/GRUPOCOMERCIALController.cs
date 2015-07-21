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
    public class GRUPOCOMERCIALController : Controller
    {
        private Entities db = new Entities();

        // GET: GRUPOCOMERCIALs
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "GrupoComercial";
            return View(db.GRUPOCOMERCIAL.ToList());
        }

        // GET: GRUPOCOMERCIALs/Details/5
       
        public ActionResult Details(string id)
        {
            ViewBag.MenuActivo = "GrupoComercial";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRUPOCOMERCIAL gRUPOCOMERCIAL = db.GRUPOCOMERCIAL.Find(id);
            if (gRUPOCOMERCIAL == null)
            {
                return HttpNotFound();
            }
            return View(gRUPOCOMERCIAL);
        }

        // GET: GRUPOCOMERCIALs/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "GrupoComercial";
            return View();
        }

        // POST: GRUPOCOMERCIALs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "GRUPOCOM,DESCRIPGRUPO")] GRUPOCOMERCIAL gRUPOCOMERCIAL)
        {
            ViewBag.MenuActivo = "GrupoComercial";
            if (ModelState.IsValid)
            {
                db.GRUPOCOMERCIAL.Add(gRUPOCOMERCIAL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gRUPOCOMERCIAL);
        }

        // GET: GRUPOCOMERCIALs/Edit/5
       
        public ActionResult Edit(string id)
        {
            ViewBag.MenuActivo = "GrupoComercial";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRUPOCOMERCIAL gRUPOCOMERCIAL = db.GRUPOCOMERCIAL.Find(id);
            if (gRUPOCOMERCIAL == null)
            {
                return HttpNotFound();
            }
            return View(gRUPOCOMERCIAL);
        }

        // POST: GRUPOCOMERCIALs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "GRUPOCOM,DESCRIPGRUPO")] GRUPOCOMERCIAL gRUPOCOMERCIAL)
        {
            ViewBag.MenuActivo = "GrupoComercial";
            if (ModelState.IsValid)
            {
                db.Entry(gRUPOCOMERCIAL).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gRUPOCOMERCIAL);
        }

        // POST: ESPECIE/Delete/5
        [HttpPost]
       
        public ActionResult Delete(string id)
        {
            try
            {
                ViewBag.MenuActivo = "GrupoComercial";
                GRUPOCOMERCIAL gRUPOCOMERCIAL = db.GRUPOCOMERCIAL.Find(id);
                db.GRUPOCOMERCIAL.Remove(gRUPOCOMERCIAL);
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
