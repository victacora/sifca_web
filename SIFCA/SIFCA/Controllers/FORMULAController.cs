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
    public class FORMULAController : Controller
    {
        private Entities db = new Entities();

        // GET: FORMULA
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Formula";
            var fORMULA = db.FORMULA.Include(f => f.TIPOFORMULA);
            return View(fORMULA.ToList());
        }

        // GET: FORMULA/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "Formula";
            ViewBag.NROTIPOFORMULA = new SelectList(db.TIPOFORMULA, "NROTIPOFORMULA", "NOMBRETIPO");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FORMULA fORMULA = db.FORMULA.Find(id);
            if (fORMULA == null)
            {
                return HttpNotFound();
            }
            return View(fORMULA);
        }

        // GET: FORMULA/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "Formula";
            ViewBag.NROTIPOFORMULA = new SelectList(db.TIPOFORMULA, "NROTIPOFORMULA", "NOMBRETIPO");
            return View();
        }

        // POST: FORMULA/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NROFORMULA,NROTIPOFORMULA,EXPRESION,NOMBRE,DESCRIPCION")] FORMULA fORMULA)
        {
            ViewBag.MenuActivo = "Formula";
            if (ModelState.IsValid)
            {
                fORMULA.NROFORMULA = Guid.NewGuid();
                db.FORMULA.Add(fORMULA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NROTIPOFORMULA = new SelectList(db.TIPOFORMULA, "NROTIPOFORMULA", "NOMBRETIPO", fORMULA.NROTIPOFORMULA);
            return View(fORMULA);
        }

        // GET: FORMULA/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Formula";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FORMULA fORMULA = db.FORMULA.Find(id);
            if (fORMULA == null)
            {
                return HttpNotFound();
            }
            ViewBag.NROTIPOFORMULA = new SelectList(db.TIPOFORMULA, "NROTIPOFORMULA", "NOMBRETIPO", fORMULA.NROTIPOFORMULA);
            return View(fORMULA);
        }

        // POST: FORMULA/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NROFORMULA,NROTIPOFORMULA,EXPRESION,NOMBRE,DESCRIPCION")] FORMULA fORMULA)
        {
            ViewBag.MenuActivo = "Formula";
            if (ModelState.IsValid)
            {
                db.Entry(fORMULA).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NROTIPOFORMULA = new SelectList(db.TIPOFORMULA, "NROTIPOFORMULA", "NOMBRETIPO", fORMULA.NROTIPOFORMULA);
            return View(fORMULA);
        }

        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "Formula";
                FORMULA fORMULA = db.FORMULA.Find(id);
                db.FORMULA.Remove(fORMULA);
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
