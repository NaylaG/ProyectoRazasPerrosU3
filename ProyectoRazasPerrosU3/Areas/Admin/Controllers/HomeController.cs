using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProyectoRazasPerrosU3.Models;
using ProyectoRazasPerrosU3.Repositories;


namespace ProyectoRazasPerrosU3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        sistem14_razasContext context;
        public IWebHostEnvironment Enviroment { get; set; }
        public HomeController(sistem14_razasContext ctx,IWebHostEnvironment env)
        {
            context = ctx;
            Enviroment = env;
        }
        public IActionResult Index()
        {
            Repository<Razas> repos = new Repository<Razas>(context);

            return View(repos.GetAll().OrderBy(x=>x.Nombre));
        }
        public IActionResult Agregar()
        {
            Razas r = new Razas();

            return View(r);
        }

        [HttpPost]
        public IActionResult Agregar(Razas r)
        {
            try
            {
                Repository<Razas> repos = new Repository<Razas>(context);
                repos.Insert(r);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception error)
            {
                ModelState.AddModelError("",error.Message);
                return View(r);
            }
           
        }

        [HttpPost]
        public IActionResult Editar(int id)
        {
            return View();
        }
        public IActionResult Editar()
        {
            return View();
        }

        
        public IActionResult Eliminar(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Eliminar()
        {
            return View();
        }
    }
}
