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
    public class PROYECTOController : Controller
    {
        private Entities db = new Entities();

        // GET: PROYECTO
       
        public ActionResult Index()
        {
            ViewBag.MenuActivo = "Proyecto";
            var pROYECTO = db.PROYECTO.Include(p => p.OBJETIVOINVENTARIO).Include(p => p.USUARIO);
            return View(pROYECTO.ToList());
        }

        // GET: PROYECTO/Details/5
       
        public ActionResult Details(Guid? id)
        {
            ViewBag.MenuActivo = "Proyecto";

            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectoViewModel pVW = new ProyectoViewModel();
            PROYECTO proyecto = db.PROYECTO.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            else
            {
                pVW.Proyecto = proyecto;
                List<Guid> especiesSeleccionadas = proyecto.ESPECIE.Select(e => e.CODESP).ToList<Guid>();
                pVW.Especies = db.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP, Familia = e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = (especiesSeleccionadas.Contains(e.CODESP) ? true : false) }).ToList();
                foreach (var estrato in db.ESTRATO)
                {
                    var estratoProyecto = proyecto.LISTADODEESTRATOS.FirstOrDefault(e => e.CODEST == estrato.CODEST);
                    if (estratoProyecto != null)
                    {
                        pVW.Estratos.Add(new EstratoViewModel() { codEstrato = estrato.CODEST, Nombre = estrato.DESCRIPESTRATO, TamanioMuestra = (estratoProyecto.TAMANIOMUESTRA != null ? (double)estratoProyecto.TAMANIOMUESTRA : 0), Peso = (estratoProyecto.PESO != null ? (double)estratoProyecto.PESO : 0), Seleccionar = true });
                    }
                    else
                    {
                        pVW.Estratos.Add(new EstratoViewModel() { codEstrato = estrato.CODEST, Nombre = estrato.DESCRIPESTRATO, TamanioMuestra = 0, Peso = 0, Seleccionar = false });
                    }
                }
                List<Guid> formulasSeleccionadas = proyecto.FORMULA.Select(e => e.NROFORMULA).ToList<Guid>();
                pVW.Formulas = db.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar = (formulasSeleccionadas.Contains(e.NROFORMULA) ? true : false) }).ToList();
                List<int> localidadesSeleccionadas = proyecto.LOCALIDAD.Select(e => e.CODLOCALIDAD).ToList<int>();
                pVW.Localidades = db.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = (localidadesSeleccionadas.Contains(e.CODLOCALIDAD) ? true : false) }).ToList();
                List<Guid> tipoLineasSeleccionadas = proyecto.TIPOLINEAINVENTARIO.Select(e => e.NROTIPOLINEAINV).ToList<Guid>();
                pVW.TipoLineaInventario = db.TIPOLINEAINVENTARIO.Select(e => new TipoLineaInventarioViewModel() { codTipoLineaInventario = e.NROTIPOLINEAINV, Nombre = e.NOMBRE, Seleccionar = (tipoLineasSeleccionadas.Contains(e.NROTIPOLINEAINV) ? true : false) }).ToList();
                foreach (var costo in db.COSTO)
                {
                    var costoProyecto = proyecto.LISTADODECOSTOS.FirstOrDefault(c => c.NROCOSTO == costo.NROCOSTO);
                    if (costoProyecto != null)
                    {
                        pVW.Costos.Add(new CostoViewModel() { codCosto = costo.NROCOSTO, Nombre = costo.NOMBRE, Valor = costoProyecto.VALOR, Seleccionar = true });
                    }
                    else
                    {
                        pVW.Costos.Add(new CostoViewModel() { codCosto = costo.NROCOSTO, Nombre = costo.NOMBRE, Valor = 0, Seleccionar = false });
                    }
                }
            }
            return View(pVW);
    }

    // GET: PROYECTO/Create
       
    public ActionResult Create()
    {
        ViewBag.MenuActivo = "Proyecto";

        ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
        List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
        seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
        seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
        ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

        List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
        tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
        tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
        tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
        tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
        tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
        ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

        List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
        tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
        tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
        tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
        ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

        List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
        tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
        tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
        tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
        ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

        ProyectoViewModel pVW = new ProyectoViewModel();
        pVW.Proyecto.FECHA = DateTime.Now;
        pVW.Especies = db.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP,Familia=e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = false }).ToList();
        pVW.Estratos = db.ESTRATO.Select(e => new EstratoViewModel() { codEstrato = e.CODEST, Nombre = e.DESCRIPESTRATO,TamanioMuestra=0,Peso=0, Seleccionar = false }).ToList();
        pVW.Formulas = db.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
        pVW.Localidades = db.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
        pVW.TipoLineaInventario = db.TIPOLINEAINVENTARIO.Select(e => new TipoLineaInventarioViewModel() { codTipoLineaInventario = e.NROTIPOLINEAINV, Nombre = e.NOMBRE, Seleccionar = false }).ToList();
        pVW.Costos = db.COSTO.Select(e => new CostoViewModel() { codCosto = e.NROCOSTO, Nombre = e.NOMBRE,Valor=0, Seleccionar = false }).ToList();
        return View(pVW);
        }

        // POST: PROYECTO/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProyectoViewModel pVW)
        {
            ViewBag.MenuActivo = "Proyecto";
            pVW.Proyecto.NROPROY = Guid.NewGuid();
            if (Session["USUARIO"] == null)
            {
                return RedirectToAction("Login", "Account", null);
            }
            else pVW.Proyecto.NROUSUARIO = ((USUARIO)Session["USUARIO"]).NROUSUARIO;
            if (ModelState.IsValid)
            {
                pVW.Proyecto.LISTADODECOSTOS = pVW.Costos.Where(c => c.Seleccionar).Select(c => new LISTADODECOSTOS() { NROCOSTO = c.codCosto, VALOR = c.Valor }).ToList<LISTADODECOSTOS>();
                pVW.Proyecto.LISTADODEESTRATOS = pVW.Estratos.Where(e => e.Seleccionar).Select(e => new LISTADODEESTRATOS() { CODEST = e.codEstrato, NROPROY = pVW.Proyecto.NROPROY, PESO = e.Peso,TAMANIOMUESTRA=e.TamanioMuestra }).ToList<LISTADODEESTRATOS>();

                List<Guid> tipoLineaInvSeleccionadas = pVW.TipoLineaInventario.Where(tVW => tVW.Seleccionar).Select(tVW => tVW.codTipoLineaInventario).ToList<Guid>();
                pVW.Proyecto.TIPOLINEAINVENTARIO = db.TIPOLINEAINVENTARIO.Where(t => tipoLineaInvSeleccionadas.Contains(t.NROTIPOLINEAINV)).ToList<TIPOLINEAINVENTARIO>();

                List<Guid> especiesSeleccionadas = pVW.Especies.Where(eVW => eVW.Seleccionar).Select(eVW => eVW.codEspecie).ToList<Guid>();
                pVW.Proyecto.ESPECIE = db.ESPECIE.Where(e => especiesSeleccionadas.Contains(e.CODESP)).ToList<ESPECIE>();

                List<int> localidadesSeleccionadas = pVW.Localidades.Where(lVW => lVW.Seleccionar).Select(lVW => lVW.codLocalidad).ToList<int>();
                pVW.Proyecto.LOCALIDAD = db.LOCALIDAD.Where(l => localidadesSeleccionadas.Contains(l.CODLOCALIDAD)).ToList<LOCALIDAD>();

                db.PROYECTO.Add(pVW.Proyecto);
                db.SaveChanges();
            
                return RedirectToAction("Index");
            }

            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
 
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            return View(pVW);
        }

        // GET: PROYECTO/Edit/5
       
        public ActionResult Edit(Guid? id)
        {
            ViewBag.MenuActivo = "Proyecto";

            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "NOMBRETIPOINV", "DESCRIPOBJETINV");
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectoViewModel pVW = new ProyectoViewModel();
            PROYECTO proyecto = db.PROYECTO.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            else
            {
                pVW.Proyecto = proyecto;
                List<Guid> especiesSeleccionadas = proyecto.ESPECIE.Select(e => e.CODESP).ToList<Guid>();
                pVW.Especies = db.ESPECIE.Select(e => new EspecieViewModel() { codEspecie = e.CODESP, Familia = e.FAMILIA, NombreCientifico = e.NOMCIENTIFICO, NombreComun = e.NOMCOMUN, Seleccionar = (especiesSeleccionadas.Contains(e.CODESP)?true:false) }).ToList();
                foreach (var estrato in db.ESTRATO)
                {
                    var estratoProyecto = proyecto.LISTADODEESTRATOS.FirstOrDefault(e => e.CODEST == estrato.CODEST);
                    if (estratoProyecto != null)
                    {
                        pVW.Estratos.Add(new EstratoViewModel() { codEstrato = estrato.CODEST, Nombre = estrato.DESCRIPESTRATO, TamanioMuestra = (estratoProyecto.TAMANIOMUESTRA!=null?(double)estratoProyecto.TAMANIOMUESTRA:0), Peso = (estratoProyecto.PESO!=null?(double)estratoProyecto.PESO:0), Seleccionar = true });
                    }
                    else
                    {
                        pVW.Estratos.Add(new EstratoViewModel() { codEstrato = estrato.CODEST, Nombre = estrato.DESCRIPESTRATO, TamanioMuestra=0, Peso=0, Seleccionar = false });
                    }
                }
                List<Guid> formulasSeleccionadas = proyecto.FORMULA.Select(e => e.NROFORMULA).ToList<Guid>();
                pVW.Formulas = db.FORMULA.Select(e => new FormulaViewModel() { codFormula = e.NROFORMULA, Nombre = e.NOMBRE, Seleccionar=(formulasSeleccionadas.Contains(e.NROFORMULA) ? true : false) }).ToList();
                List<int> localidadesSeleccionadas = proyecto.LOCALIDAD.Select(e => e.CODLOCALIDAD).ToList<int>();
                pVW.Localidades = db.LOCALIDAD.Select(e => new LocalidadViewModel() { codLocalidad = e.CODLOCALIDAD, Nombre = e.NOMBRE, Seleccionar = (localidadesSeleccionadas.Contains(e.CODLOCALIDAD) ? true : false) }).ToList();
                List<Guid> tipoLineasSeleccionadas = proyecto.TIPOLINEAINVENTARIO.Select(e => e.NROTIPOLINEAINV).ToList<Guid>();
                pVW.TipoLineaInventario = db.TIPOLINEAINVENTARIO.Select(e => new TipoLineaInventarioViewModel() { codTipoLineaInventario = e.NROTIPOLINEAINV, Nombre = e.NOMBRE, Seleccionar = (tipoLineasSeleccionadas.Contains(e.NROTIPOLINEAINV) ? true : false) }).ToList();
                foreach (var costo in db.COSTO)
                {
                    var costoProyecto=proyecto.LISTADODECOSTOS.FirstOrDefault(c => c.NROCOSTO == costo.NROCOSTO);
                    if (costoProyecto != null)
                    {
                        pVW.Costos.Add(new CostoViewModel() { codCosto = costo.NROCOSTO, Nombre = costo.NOMBRE, Valor = costoProyecto.VALOR, Seleccionar = true });
                    }
                    else 
                    {
                        pVW.Costos.Add(new CostoViewModel() { codCosto = costo.NROCOSTO, Nombre = costo.NOMBRE, Valor = 0, Seleccionar = false });
                    }
                }
            }
            return View(pVW);
        }

        // POST: PROYECTO/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(ProyectoViewModel pVW)
        {
            ViewBag.MenuActivo = "Proyecto";      
            PROYECTO proyecto = db.PROYECTO.Find(pVW.Proyecto.NROPROY);
            if (ModelState.IsValid && proyecto != null)
            {   
                //Procesar si no esta en la lista agrgar si esta editar
                //copiar datos del proyecto para editar
                proyecto.NOMBRE = pVW.Proyecto.NOMBRE;
                proyecto.NOMBRETIPOINV = pVW.Proyecto.NOMBRETIPOINV;
                proyecto.FECHA = pVW.Proyecto.FECHA;
                proyecto.DESCRIPCION = pVW.Proyecto.DESCRIPCION;
                proyecto.TIPOPROYECTO = pVW.Proyecto.TIPOPROYECTO;
                proyecto.PRESUPUESTO = pVW.Proyecto.PRESUPUESTO;
                proyecto.PLAZO = pVW.Proyecto.PLAZO;
                proyecto.TIPOPLAZO = pVW.Proyecto.TIPOPLAZO;
                proyecto.NUMEROPERSONAS = pVW.Proyecto.NUMEROPERSONAS;
                proyecto.TIPODISENIOMUESTRAL = pVW.Proyecto.TIPODISENIOMUESTRAL;
                proyecto.SELECCIONALEATORIA = pVW.Proyecto.SELECCIONALEATORIA;
                proyecto.CONFIANZA = pVW.Proyecto.CONFIANZA;
                proyecto.NUMEROPARCELAS = pVW.Proyecto.NUMEROPARCELAS;
                proyecto.NUMEROPARCELASAMUESTREAR = pVW.Proyecto.NUMEROPARCELASAMUESTREAR;
                proyecto.TAMANIOPARCELA = pVW.Proyecto.TAMANIOPARCELA;
                proyecto.AREATOTAL = pVW.Proyecto.AREATOTAL;
                proyecto.INTMUESTREO = pVW.Proyecto.INTMUESTREO;
                proyecto.AREAREGENERACION = pVW.Proyecto.AREAREGENERACION;
                proyecto.FACTORDEFORMA = pVW.Proyecto.FACTORDEFORMA;

                foreach (var especie in pVW.Especies)
                {
                    ESPECIE especieProyecto = proyecto.ESPECIE.FirstOrDefault(e => e.CODESP == especie.codEspecie);
                    if (especie.Seleccionar)
                    {
                        if (especieProyecto == null)
                        {
                            especieProyecto = db.ESPECIE.FirstOrDefault(e => e.CODESP == especie.codEspecie);
                            if (especieProyecto == null)
                            {
                                proyecto.ESPECIE.Add(especieProyecto);
                            }
                        }
                    }
                    else
                    {
                        if (especieProyecto != null)
                        {
                            db.Entry(especieProyecto).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }
                
                foreach (var estrato in pVW.Estratos)
                {
                    LISTADODEESTRATOS estratoProyecto = proyecto.LISTADODEESTRATOS.FirstOrDefault(e => e.CODEST == estrato.codEstrato);
                    if (estrato.Seleccionar)
                    {
                        if (estratoProyecto != null)
                        {
                            estratoProyecto.TAMANIOMUESTRA = estrato.TamanioMuestra;
                            estratoProyecto.PESO = estrato.Peso;
                            db.Entry(estratoProyecto).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            proyecto.LISTADODEESTRATOS.Add(new LISTADODEESTRATOS() { NROPROY = proyecto.NROPROY, CODEST = estrato.codEstrato, TAMANIOMUESTRA = estrato.TamanioMuestra, PESO = estrato.Peso });
                        }
                    }
                    else
                    {
                        if (estratoProyecto != null)
                        {
                            db.Entry(estratoProyecto).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }

                foreach (var tipoLineaInv in pVW.TipoLineaInventario)
                {
                    TIPOLINEAINVENTARIO tipoLineaInvProyecto = proyecto.TIPOLINEAINVENTARIO.FirstOrDefault(t => t.NROTIPOLINEAINV == tipoLineaInv.codTipoLineaInventario);
                    if (tipoLineaInv.Seleccionar)
                    {
                        if (tipoLineaInvProyecto == null)
                        {
                            tipoLineaInvProyecto = db.TIPOLINEAINVENTARIO.FirstOrDefault(t => t.NROTIPOLINEAINV == tipoLineaInv.codTipoLineaInventario);
                            if (tipoLineaInvProyecto == null)
                            {
                                proyecto.TIPOLINEAINVENTARIO.Add(tipoLineaInvProyecto);
                            }
                        }
                    }
                    else
                    {
                        if (tipoLineaInvProyecto != null)
                        {
                            db.Entry(tipoLineaInvProyecto).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }

                foreach (var formula in pVW.Formulas)
                {
                    FORMULA formulaProyecto = proyecto.FORMULA.FirstOrDefault(f => f.NROFORMULA == formula.codFormula);
                    if (formula.Seleccionar)
                    {
                        if (formulaProyecto == null)
                        {
                            formulaProyecto = db.FORMULA.FirstOrDefault(f => f.NROFORMULA == formula.codFormula);
                            if (formulaProyecto == null)
                            {
                                proyecto.FORMULA.Add(formulaProyecto);
                            }
                        }
                    }
                    else
                    {
                        if (formulaProyecto != null)
                        {
                            db.Entry(formulaProyecto).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }

                foreach (var costo in pVW.Costos)
                {
                    LISTADODECOSTOS costoProyecto = proyecto.LISTADODECOSTOS.FirstOrDefault(c => c.NROCOSTO == costo.codCosto);
                    if (costo.Seleccionar)
                    {
                        if (costoProyecto != null)
                        {
                            costoProyecto.VALOR = costo.Valor;
                            db.Entry(costoProyecto).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            proyecto.LISTADODECOSTOS.Add(new LISTADODECOSTOS() { NROPROY = proyecto.NROPROY, NROCOSTO = costo.codCosto, VALOR = costo.Valor });
                        }
                    }
                    else
                    {
                        if (costoProyecto != null)
                        {
                            db.Entry(costoProyecto).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }
                

                foreach (var localidad in pVW.Localidades)
                {
                    LOCALIDAD localidadProyecto = proyecto.LOCALIDAD.FirstOrDefault(l => l.CODLOCALIDAD == localidad.codLocalidad);
                    if (localidad.Seleccionar)
                    {
                        if (localidadProyecto == null)
                        {
                            localidadProyecto = db.LOCALIDAD.FirstOrDefault(l => l.CODLOCALIDAD == localidad.codLocalidad);
                            if (localidadProyecto == null)
                            {
                                proyecto.LOCALIDAD.Add(localidadProyecto);
                            }
                        }
                    }
                    else
                    {
                        if (localidadProyecto != null)
                        {
                            db.Entry(localidadProyecto).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }
                }

                db.Entry(proyecto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.tipoInventario = new SelectList(db.OBJETIVOINVENTARIO, "tipoInventario", "DESCRIPOBJETINV", pVW.Proyecto.NOMBRETIPOINV);
 
            List<KeyValuePair<string, string>> seleccionarSiNo = new List<KeyValuePair<string, string>>();
            seleccionarSiNo.Add(new KeyValuePair<string, string>("S", "Si"));
            seleccionarSiNo.Add(new KeyValuePair<string, string>("N", "No"));
            ViewBag.seleccionarSiNo = new SelectList(seleccionarSiNo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoPlazo = new List<KeyValuePair<string, string>>();
            tipoPlazo.Add(new KeyValuePair<string, string>("H", "Horas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("D", "Dias"));
            tipoPlazo.Add(new KeyValuePair<string, string>("S", "Semanas"));
            tipoPlazo.Add(new KeyValuePair<string, string>("M", "Meses"));
            tipoPlazo.Add(new KeyValuePair<string, string>("A", "Años"));
            ViewBag.tipoPlazo = new SelectList(tipoPlazo, "Key", "Value");

            List<KeyValuePair<string, string>> tipoDisenioMuetral = new List<KeyValuePair<string, string>>();
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("S", "Simple"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("E", "Estratificado"));
            tipoDisenioMuetral.Add(new KeyValuePair<string, string>("B", "Bietapico"));
            ViewBag.tipoDisenioMuetral = new SelectList(tipoDisenioMuetral, "Key", "Value");

            List<KeyValuePair<string, string>> tipoProyecto = new List<KeyValuePair<string, string>>();
            tipoProyecto.Add(new KeyValuePair<string, string>("IN", "Independiente"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CO", "Contenido"));
            tipoProyecto.Add(new KeyValuePair<string, string>("CR", "Contenedor"));
            ViewBag.tipoProyecto = new SelectList(tipoProyecto, "Key", "Value");

            return View(pVW);
        }

        // POST: PROYECTO/Delete/5
        [HttpPost]
       
        public ActionResult Delete(Guid id)
        {
            try
            {
                ViewBag.MenuActivo = "Proyecto";
                PROYECTO pROYECTO = db.PROYECTO.Find(id);
                db.PROYECTO.Remove(pROYECTO);
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
