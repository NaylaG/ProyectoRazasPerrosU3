using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProyectoRazasPerrosU3.Models;
using ProyectoRazasPerrosU3.Models.ViewModels;
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
            InfoPerroViewModel vm = new InfoPerroViewModel();
           
            return View(vm);
        }

        [HttpPost]
        public IActionResult Agregar(InfoPerroViewModel vm)
        {
            try
            {
                if(vm.Archivo==null)
                {
                    ModelState.AddModelError("", "Debe seleccionar una imagen para la raza");
                    return View(vm);
                }
                else
                {
                    if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un archivo jpg menor a 2MB");
                      
                        return View(vm);
                    }
                }
               
                Repository<Razas> repos = new Repository<Razas>(context);
                
                repos.Insert(vm.Raza);
                if (vm.Archivo != null)
                {
                    FileStream fs = new FileStream(Enviroment.WebRootPath + "/imgs_perros/" + vm.Raza.Id + "_0.jpg", FileMode.Create);
                    vm.Archivo.CopyTo(fs);
                    fs.Close();
                }


                return RedirectToAction("Index", "Home");
            }
            catch (Exception error)
            {
                ModelState.AddModelError("",error.Message);
                return View(vm);
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
