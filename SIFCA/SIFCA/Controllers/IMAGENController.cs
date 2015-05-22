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
    public class IMAGENController : Controller
    {
        private Entities db = new Entities();

        // GET: IMAGEN
       
        public ActionResult Index()
        {
            var iMAGEN = db.IMAGEN.Include(i => i.ESPECIE);
            return View(iMAGEN.ToList());
        }

        // GET: IMAGEN/Details/5
       
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IMAGEN iMAGEN = db.IMAGEN.Find(id);
            if (iMAGEN == null)
            {
                return HttpNotFound();
            }
            return View(iMAGEN);
        }

        // GET: IMAGEN/Create
       
        public ActionResult Create()
        {
            ViewBag.CODESP = new SelectList(db.ESPECIE, "CODESP", "GRUPOCOM");
            return View();
        }

        // POST: IMAGEN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NROIMAGEN,CODESP,DESCRIPCION,NOMBRE,RUTA")] IMAGEN iMAGEN)
        {
            if (ModelState.IsValid)
            {
                iMAGEN.NROIMAGEN = Guid.NewGuid();
                db.IMAGEN.Add(iMAGEN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CODESP = new SelectList(db.ESPECIE, "CODESP", "GRUPOCOM", iMAGEN.CODESP);
            return View(iMAGEN);
        }

        // GET: IMAGEN/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IMAGEN iMAGEN = db.IMAGEN.Find(id);
            if (iMAGEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.CODESP = new SelectList(db.ESPECIE, "CODESP", "GRUPOCOM", iMAGEN.CODESP);
            return View(iMAGEN);
        }

        // POST: IMAGEN/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NROIMAGEN,CODESP,DESCRIPCION,NOMBRE,RUTA")] IMAGEN iMAGEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iMAGEN).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CODESP = new SelectList(db.ESPECIE, "CODESP", "GRUPOCOM", iMAGEN.CODESP);
            return View(iMAGEN);
        }

        // GET: IMAGEN/Delete/5
       
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IMAGEN iMAGEN = db.IMAGEN.Find(id);
            if (iMAGEN == null)
            {
                return HttpNotFound();
            }
            return View(iMAGEN);
        }

        // POST: IMAGEN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
       
        public ActionResult DeleteConfirmed(Guid id)
        {
            IMAGEN iMAGEN = db.IMAGEN.Find(id);
            db.IMAGEN.Remove(iMAGEN);
            db.SaveChanges();
            return RedirectToAction("Index");
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
