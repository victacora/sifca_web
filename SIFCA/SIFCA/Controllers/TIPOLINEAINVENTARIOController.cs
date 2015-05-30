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
    public class TIPOLINEAINVENTARIOController : Controller
    {
        private Entities db = new Entities();

        // GET: TIPOLINEAINVENTARIO
        public ActionResult Index()
        {
            return View(db.TIPOLINEAINVENTARIO.ToList());
        }

        // GET: TIPOLINEAINVENTARIO/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIPOLINEAINVENTARIO tIPOLINEAINVENTARIO = db.TIPOLINEAINVENTARIO.Find(id);
            if (tIPOLINEAINVENTARIO == null)
            {
                return HttpNotFound();
            }
            return View(tIPOLINEAINVENTARIO);
        }

        // GET: TIPOLINEAINVENTARIO/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TIPOLINEAINVENTARIO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NROTIPOLINEAINV,NOMBRE,DESCRIPCION,REQUIEREESPECIE")] TIPOLINEAINVENTARIO tIPOLINEAINVENTARIO)
        {
            if (ModelState.IsValid)
            {
                tIPOLINEAINVENTARIO.NROTIPOLINEAINV = Guid.NewGuid();
                db.TIPOLINEAINVENTARIO.Add(tIPOLINEAINVENTARIO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tIPOLINEAINVENTARIO);
        }

        // GET: TIPOLINEAINVENTARIO/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIPOLINEAINVENTARIO tIPOLINEAINVENTARIO = db.TIPOLINEAINVENTARIO.Find(id);
            if (tIPOLINEAINVENTARIO == null)
            {
                return HttpNotFound();
            }
            return View(tIPOLINEAINVENTARIO);
        }

        // POST: TIPOLINEAINVENTARIO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NROTIPOLINEAINV,NOMBRE,DESCRIPCION,REQUIEREESPECIE")] TIPOLINEAINVENTARIO tIPOLINEAINVENTARIO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tIPOLINEAINVENTARIO).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tIPOLINEAINVENTARIO);
        }

        // POST: TIPOLINEAINVENTARIO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TIPOLINEAINVENTARIO tIPOLINEAINVENTARIO = db.TIPOLINEAINVENTARIO.Find(id);
            db.TIPOLINEAINVENTARIO.Remove(tIPOLINEAINVENTARIO);
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
