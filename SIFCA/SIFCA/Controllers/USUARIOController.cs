using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIFCA.Models;
using System.Configuration;
using SIFCA.Helpers;

namespace SIFCA.Controllers
{
    public class USUARIOController : Controller
    {
        private Entities db = new Entities();

        // GET: USUARIO
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Usuario";
            return View(db.USUARIO.ToList());
        }

        // GET: USUARIO/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "Usuario";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<KeyValuePair<string, string>> tipoUsuario = new List<KeyValuePair<string, string>>();
            tipoUsuario.Add(new KeyValuePair<string, string>("AD", "Administrador"));
            tipoUsuario.Add(new KeyValuePair<string, string>("NA", "Usuario normal"));
            ViewBag.TIPOUSUARIO = new SelectList(tipoUsuario, "Key", "Value");
                
            USUARIO uSUARIO = db.USUARIO.Find(id);
            string key = ConfigurationManager.AppSettings["Key"];
            uSUARIO.CONTRASENA = AuthenticatorHelper.Decrypt(uSUARIO.CONTRASENA, key);
            if (uSUARIO == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIO);
        }

        // GET: USUARIO/Create
       
        public ActionResult Create()
        {
            ViewBag.MenuActivo = "Usuario";
            List<KeyValuePair<string, string>> tipoUsuario = new List<KeyValuePair<string, string>>();
            tipoUsuario.Add(new KeyValuePair<string, string>("AD", "Administrador"));
            tipoUsuario.Add(new KeyValuePair<string, string>("NA", "Usuario normal"));
            ViewBag.TIPOUSUARIO = new SelectList(tipoUsuario, "Key", "Value");
            return View();
        }

        // POST: USUARIO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "NROUSUARIO,NOMBRES,APELLIDOS,NOMBREUSUARIO,CONTRASENA,CEDULA,TIPOUSUARIO")] USUARIO uSUARIO)
        {
            ViewBag.MenuActivo = "Usuario";
            if (ModelState.IsValid)
            {
                uSUARIO.NROUSUARIO = Guid.NewGuid();
                string key = ConfigurationManager.AppSettings["Key"];
                uSUARIO.CONTRASENA = AuthenticatorHelper.Encrypt(uSUARIO.CONTRASENA, key);
                db.USUARIO.Add(uSUARIO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uSUARIO);
        }

        // GET: USUARIO/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Usuario";
            List<KeyValuePair<string, string>> tipoUsuario = new List<KeyValuePair<string, string>>();
            tipoUsuario.Add(new KeyValuePair<string, string>("AD", "Administrador"));
            tipoUsuario.Add(new KeyValuePair<string, string>("NA", "Usuario normal"));
            ViewBag.TIPOUSUARIO = new SelectList(tipoUsuario, "Key", "Value");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO uSUARIO = db.USUARIO.Find(id);
            string key = ConfigurationManager.AppSettings["Key"];
            uSUARIO.CONTRASENA = AuthenticatorHelper.Decrypt(uSUARIO.CONTRASENA, key);
            if (uSUARIO == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIO);
        }

        // POST: USUARIO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Edit([Bind(Include = "NROUSUARIO,NOMBRES,APELLIDOS,NOMBREUSUARIO,CONTRASENA,CEDULA,TIPOUSUARIO")] USUARIO uSUARIO)
        {
            ViewBag.MenuActivo = "Usuario";
            if (ModelState.IsValid)
            {
                string key = ConfigurationManager.AppSettings["Key"];
                uSUARIO.CONTRASENA = AuthenticatorHelper.Encrypt(uSUARIO.CONTRASENA, key);
                db.Entry(uSUARIO).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uSUARIO);
        }

        // POST: USUARIO/Delete/5
        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "Usuario";
                USUARIO uSUARIO = db.USUARIO.Find(id);
                db.USUARIO.Remove(uSUARIO);
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
