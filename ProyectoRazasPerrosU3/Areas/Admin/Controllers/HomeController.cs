using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            return View(repos.GetAll().OrderBy(x=>x.Nombre).Where(x=>x.Eliminado==1));
        }
        public IActionResult Agregar()
        {
            InfoPerroViewModel vm = new InfoPerroViewModel();
            Repository<Paises> reposPaises = new Repository<Paises>(context);
            vm.Paises = reposPaises.GetAll();
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


        public IActionResult Editar(uint id)
        {
            InfoPerroViewModel vm = new InfoPerroViewModel();
            Repository<Razas> repos = new Repository<Razas>(context);
            Repository<Paises> reposPaises = new Repository<Paises>(context);
            vm.Paises = reposPaises.GetAll();
            vm.Raza = repos.GetById(id);            
            if (vm.Raza == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (System.IO.File.Exists(Enviroment.WebRootPath + $"/imgs_perros/{vm.Raza.Id}_0.jpg"))
            {
                vm.Imagen = vm.Raza.Id + "_0.jpg";
            }
            else
            {
                vm.Imagen = "NoPhoto.jpg";
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Editar(InfoPerroViewModel vm)
        {
            if(vm.Archivo!=null)
            {
                if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo jpg de menos de 2MB.");
                    Repository<Paises> reposPaises = new Repository<Paises>(context);
                    vm.Paises = reposPaises.GetAll();
                    return View(vm);
                }
            }
            try
            {
                Repository<Razas> repos = new Repository<Razas>(context);
                var original = repos.GetById(vm.Raza.Id);
                if(original!=null)
                {
                    original.AlturaMax = vm.Raza.AlturaMax;
                    original.Nombre = vm.Raza.Nombre;
                    original.Descripcion = vm.Raza.Descripcion;
                    original.OtrosNombres = vm.Raza.OtrosNombres;
                    original.PesoMax = vm.Raza.PesoMax;
                    original.EsperanzaVida = vm.Raza.EsperanzaVida;
                    original.Eliminado = vm.Raza.Eliminado;
                    original.IdPaisNavigation.Nombre = vm.Raza.IdPaisNavigation.Nombre;
                    original.Caracteristicasfisicas.Patas = vm.Raza.Caracteristicasfisicas.Patas;
                    original.Caracteristicasfisicas.Cola = vm.Raza.Caracteristicasfisicas.Cola;
                    original.Caracteristicasfisicas.Hocico = vm.Raza.Caracteristicasfisicas.Hocico;
                    original.Caracteristicasfisicas.Pelo = vm.Raza.Caracteristicasfisicas.Pelo;
                    original.Caracteristicasfisicas.Color = vm.Raza.Caracteristicasfisicas.Color;
                    original.Estadisticasraza.NivelEnergia = vm.Raza.Estadisticasraza.NivelEnergia;
                    original.Estadisticasraza.FacilidadEntrenamiento = vm.Raza.Estadisticasraza.FacilidadEntrenamiento;
                    original.Estadisticasraza.EjercicioObligatorio = vm.Raza.Estadisticasraza.EjercicioObligatorio;
                    original.Estadisticasraza.AmistadDesconocidos = vm.Raza.Estadisticasraza.AmistadDesconocidos;
                    original.Estadisticasraza.NecesidadCepillado = vm.Raza.Estadisticasraza.NecesidadCepillado;
                }
                if(vm.Archivo!=null)
                {
                    FileStream fs = new FileStream(Enviroment.WebRootPath + $"/imgs_perros/{vm.Raza.Id}_0.jpg", FileMode.Create);
                    vm.Archivo.CopyTo(fs);
                    fs.Close();
                }
                return RedirectToAction("Index", "Home");

            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                Repository<Paises> reposPaises = new Repository<Paises>(context);
                vm.Paises = reposPaises.GetAll();
                return View(vm);             
            }            
        }

        
        public IActionResult Eliminar(uint id)
        {
            Repository<Razas> repos = new Repository<Razas>(context);
            InfoPerroViewModel raza = new InfoPerroViewModel();
            raza.Raza = repos.GetById(id);
            if (raza != null)
                return  View(raza);
            else 
                return RedirectToAction("Index", "Home");
           
        }

        [HttpPost]
        public IActionResult Eliminar(InfoPerroViewModel vm)
        {
            try
            {
                Repository<Razas> repos = new Repository<Razas>(context);
                InfoPerroViewModel temporal = new InfoPerroViewModel();

                temporal.Raza = repos.GetById(vm.Raza.Id);
                temporal.Raza.Eliminado = 0;
                repos.Update(temporal.Raza);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View(vm);
            }            
        }
    }
}
