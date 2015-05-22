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
    public class TIPOFORMULAController : Controller
    {
        private Entities db = new Entities();

        // GET: TIPOFORMULAs
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "TipoFormula";
            return View(db.TIPOFORMULA.ToList());
        }

        // GET: TIPOFORMULAs/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "TipoFormula";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIPOFORMULA tIPOFORMULA = db.TIPOFORMULA.Find(id);
            if (tIPOFORMULA == null)
            {
                return HttpNotFound();
            }
            return View(tIPOFORMULA);
        }

        // GET: TIPOFORMULAs/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "TipoFormula";
            return View();
        }

        // POST: TIPOFORMULAs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NROTIPOFORMULA,NOMBRETIPO,DESCRIPCION")] TIPOFORMULA tIPOFORMULA)
        {
            ViewBag.MenuActivo = "TipoFormula";
            if (ModelState.IsValid)
            {
                tIPOFORMULA.NROTIPOFORMULA = Guid.NewGuid();
                db.TIPOFORMULA.Add(tIPOFORMULA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tIPOFORMULA);
        }

        // GET: TIPOFORMULAs/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "TipoFormula";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIPOFORMULA tIPOFORMULA = db.TIPOFORMULA.Find(id);
            if (tIPOFORMULA == null)
            {
                return HttpNotFound();
            }
            return View(tIPOFORMULA);
        }

        // POST: TIPOFORMULAs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NROTIPOFORMULA,NOMBRETIPO,DESCRIPCION")] TIPOFORMULA tIPOFORMULA)
        {
            ViewBag.MenuActivo = "TipoFormula";
            if (ModelState.IsValid)
            {
                db.Entry(tIPOFORMULA).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tIPOFORMULA);
        }


        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "TipoFormula";
                TIPOFORMULA tIPOFORMULA = db.TIPOFORMULA.Find(id);
                db.TIPOFORMULA.Remove(tIPOFORMULA);
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
