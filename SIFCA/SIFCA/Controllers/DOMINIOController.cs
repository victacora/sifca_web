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
    public class DOMINIOController : Controller
    {
        private Entities db = new Entities();

        // GET: DOMINIOs
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Dominio";
            return View(db.DOMINIO.ToList());
        }

        // GET: DOMINIOs/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "Dominio";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMINIO dOMINIO = db.DOMINIO.Find(id);
            if (dOMINIO == null)
            {
                return HttpNotFound();
            }
            return View(dOMINIO);
        }

        // GET: DOMINIOs/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "Dominio";
            return View();
        }

        // POST: DOMINIOs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NRODOMINIO,NOMBRE,DESCRIPCION")] DOMINIO dOMINIO)
        {
            ViewBag.MenuActivo = "Dominio";
            if (ModelState.IsValid)
            {
                dOMINIO.NRODOMINIO = Guid.NewGuid();
                db.DOMINIO.Add(dOMINIO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dOMINIO);
        }

        // GET: DOMINIOs/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Dominio";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMINIO dOMINIO = db.DOMINIO.Find(id);
            if (dOMINIO == null)
            {
                return HttpNotFound();
            }
            return View(dOMINIO);
        }

        // POST: DOMINIOs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NRODOMINIO,NOMBRE,DESCRIPCION")] DOMINIO dOMINIO)
        {
            ViewBag.MenuActivo = "Dominio";
            if (ModelState.IsValid)
            {
                db.Entry(dOMINIO).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dOMINIO);
        }

        // POST: DOMINIOs/Delete/5
        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "Dominio";
                DOMINIO dOMINIO = db.DOMINIO.Find(id);
                db.DOMINIO.Remove(dOMINIO);
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
