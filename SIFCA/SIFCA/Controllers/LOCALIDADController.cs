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
    public class LOCALIDADController : Controller
    {
        private Entities db = new Entities();

        // GET: LOCALIDAD
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Localidad";
            var lOCALIDAD = db.LOCALIDAD.Include(l => l.LOCALIDAD2);
            return View(lOCALIDAD.ToList());
        }

        // GET: LOCALIDAD/Details/5
       
        public ActionResult Details(int? id)
        {
            ViewBag.MenuActivo = "Localidad";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOCALIDAD lOCALIDAD = db.LOCALIDAD.Find(id);
            if (lOCALIDAD == null)
            {
                return HttpNotFound();
            }
            List<LOCALIDAD> localidades = db.LOCALIDAD.ToList();
            ViewBag.CODLOCALIDADPADRE = new SelectList(localidades, "CODLOCALIDAD", "NOMBRE", lOCALIDAD.CODLOCALIDADPADRE);
            return View(lOCALIDAD);
        }

        // GET: LOCALIDAD/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "Localidad";
            List<LOCALIDAD> localidades = db.LOCALIDAD.ToList();
            ViewBag.CODLOCALIDADPADRE = new SelectList(localidades, "CODLOCALIDAD", "NOMBRE",0);
            return View();
        }

        // POST: LOCALIDAD/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "CODLOCALIDAD,CODLOCALIDADPADRE,NOMBRE")] LOCALIDAD lOCALIDAD)
        {
            ViewBag.MenuActivo = "Localidad";
            if (ModelState.IsValid)
            {
                db.LOCALIDAD.Add(lOCALIDAD);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<LOCALIDAD> localidades = db.LOCALIDAD.ToList();
            ViewBag.CODLOCALIDADPADRE = new SelectList(localidades, "CODLOCALIDAD", "NOMBRE",lOCALIDAD.CODLOCALIDADPADRE);
            return View(lOCALIDAD);
        }

        // GET: LOCALIDAD/Edit/5
       
        public ActionResult Edit(int? id)
        {
            ViewBag.MenuActivo = "Localidad";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOCALIDAD lOCALIDAD = db.LOCALIDAD.Find(id);
            if (lOCALIDAD == null)
            {
                return HttpNotFound();
            }
            List<LOCALIDAD> localidades = db.LOCALIDAD.ToList();
            ViewBag.CODLOCALIDADPADRE = new SelectList(localidades, "CODLOCALIDAD", "NOMBRE", lOCALIDAD.CODLOCALIDADPADRE);
            return View(lOCALIDAD);
        }

        // POST: LOCALIDAD/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "CODLOCALIDAD,CODLOCALIDADPADRE,NOMBRE")] LOCALIDAD lOCALIDAD)
        {
            ViewBag.MenuActivo = "Localidad";
            if (ModelState.IsValid)
            {
                db.Entry(lOCALIDAD).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<LOCALIDAD> localidades = db.LOCALIDAD.ToList();
            ViewBag.CODLOCALIDADPADRE = new SelectList(localidades, "CODLOCALIDAD", "NOMBRE", lOCALIDAD.CODLOCALIDADPADRE);
            return View(lOCALIDAD);
        }

        // POST: ESPECIE/Delete/5
        [HttpPost]
       
        public ActionResult Delete(int? id)
        {
            try
            {
                ViewBag.MenuActivo = "Localidad";
                LOCALIDAD lOCALIDAD= db.LOCALIDAD.Find(id);
                db.LOCALIDAD.Remove(lOCALIDAD);
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
