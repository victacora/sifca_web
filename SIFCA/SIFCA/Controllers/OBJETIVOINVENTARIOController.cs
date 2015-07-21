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
    public class OBJETIVOINVENTARIOController : Controller
    {
        private Entities db = new Entities();

        // GET: OBJETIVOINVENTARIO
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "ObjetivoInventario";
            return View(db.OBJETIVOINVENTARIO.ToList());
        }

        // GET: OBJETIVOINVENTARIO/Details/5
       
        public ActionResult Details(string id)
        {
            ViewBag.MenuActivo = "ObjetivoInventario";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBJETIVOINVENTARIO oBJETIVOINVENTARIO = db.OBJETIVOINVENTARIO.Find(id);
            if (oBJETIVOINVENTARIO == null)
            {
                return HttpNotFound();
            }
            return View(oBJETIVOINVENTARIO);
        }

        // GET: OBJETIVOINVENTARIO/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "ObjetivoInventario";
            return View();
        }

        // POST: OBJETIVOINVENTARIO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NOMBRETIPOINV,DESCRIPOBJETINV")] OBJETIVOINVENTARIO oBJETIVOINVENTARIO)
        {
            ViewBag.MenuActivo = "ObjetivoInventario";
            if (ModelState.IsValid)
            {
                db.OBJETIVOINVENTARIO.Add(oBJETIVOINVENTARIO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oBJETIVOINVENTARIO);
        }

        // GET: OBJETIVOINVENTARIO/Edit/5
       
        public ActionResult Edit(string id)
        {
            ViewBag.MenuActivo = "ObjetivoInventario";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBJETIVOINVENTARIO oBJETIVOINVENTARIO = db.OBJETIVOINVENTARIO.Find(id);
            if (oBJETIVOINVENTARIO == null)
            {
                return HttpNotFound();
            }
            return View(oBJETIVOINVENTARIO);
        }

        // POST: OBJETIVOINVENTARIO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NOMBRETIPOINV,DESCRIPOBJETINV")] OBJETIVOINVENTARIO oBJETIVOINVENTARIO)
        {
            ViewBag.MenuActivo = "ObjetivoInventario";
            if (ModelState.IsValid)
            {
                db.Entry(oBJETIVOINVENTARIO).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oBJETIVOINVENTARIO);
        }


        [HttpPost]
       
        public ActionResult Delete(string id)
        {
            try
            {
                ViewBag.MenuActivo = "ObjetivoInventario";
                OBJETIVOINVENTARIO oBJETIVOINVENTARIO = db.OBJETIVOINVENTARIO.Find(id);
                db.OBJETIVOINVENTARIO.Remove(oBJETIVOINVENTARIO);
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
