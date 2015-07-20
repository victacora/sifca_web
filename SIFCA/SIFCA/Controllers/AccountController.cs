using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SIFCA.Models;
using System.Web.Security;
using SIFCA.Helpers;
using System.Configuration;

namespace SIFCA.Controllers
{
    public class AccountController : Controller
    {
        private Entities db = new Entities();

        [AllowAnonymous]
        public ActionResult LogOff(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            FormsAuthentication.SignOut();
            Session["USUARIO"] = null;
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public USUARIO LoginUser(USUARIO user, string key)
        {
            USUARIO userData =  db.USUARIO.SingleOrDefault(u => u.NOMBREUSUARIO == user.NOMBREUSUARIO);
            if (userData != null)
            {
                if (AuthenticatorHelper.Decrypt(userData.CONTRASENA, key) == user.CONTRASENA) return userData;
            }
            return null;

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string key = ConfigurationManager.AppSettings["Key"];
            USUARIO usuario=LoginUser(new USUARIO(){CONTRASENA=model.Password,NOMBREUSUARIO=model.Login},key);
            if (usuario!=null)
            {
                //create the authentication ticket
             //   FormsAuthentication.SetAuthCookie(usuario.NROUSUARIO.ToString(), model.RememberMe);
               FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                 1,
                 usuario.NROUSUARIO.ToString(), //user id
                      DateTime.Now,
                      DateTime.Now.AddMinutes(30),  // expiry
                      model.RememberMe,  //true to remember
                      "", //roles 
                      "/"
                    );
                    //encrypt the ticket and add it to a cookie
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                    Response.Cookies.Add(cookie);
                    Session["USUARIO"] = usuario; 
                return RedirectToAction("Index", "Home");
            }
            else ModelState.AddModelError("", "Intento inválido de inicio de sesión.");
            return View(model);
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