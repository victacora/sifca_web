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
    public class ESTRATOController : Controller
    {
        private Entities db = new Entities();

        // GET: ESTRATO
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Estrato";
            return View(db.ESTRATO.ToList());
        }

        // GET: ESTRATO/Details/5
       
        public ActionResult Details(int? id)
        {
            ViewBag.MenuActivo = "Estrato";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ESTRATO eSTRATO = db.ESTRATO.Find(id);
            if (eSTRATO == null)
            {
                return HttpNotFound();
            }
            return View(eSTRATO);
        }

        // GET: ESTRATO/Create
       
        public ActionResult Create()
        {
            return View();
        }

        // POST: ESTRATO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "CODEST,DESCRIPESTRATO")] ESTRATO eSTRATO)
        {
            ViewBag.MenuActivo = "Estrato";
            if (ModelState.IsValid)
            {
                db.ESTRATO.Add(eSTRATO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eSTRATO);
        }

        // GET: ESTRATO/Edit/5
       
        public ActionResult Edit(int? id)
        {
            ViewBag.MenuActivo = "Estrato";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ESTRATO eSTRATO = db.ESTRATO.Find(id);
            if (eSTRATO == null)
            {
                return HttpNotFound();
            }
            return View(eSTRATO);
        }

        // POST: ESTRATO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "CODEST,DESCRIPESTRATO")] ESTRATO eSTRATO)
        {
            ViewBag.MenuActivo = "Estrato";
            if (ModelState.IsValid)
            {
                db.Entry(eSTRATO).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eSTRATO);
        }

        // POST: ESTRATO/Delete/5
        [HttpPost]
       
        public ActionResult Delete(int id)
        {
            ViewBag.MenuActivo = "Estrato";
            try
            {
                ESTRATO eSTRATO = db.ESTRATO.Find(id);
                db.ESTRATO.Remove(eSTRATO);
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
