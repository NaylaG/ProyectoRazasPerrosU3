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
            RazasRepository  repos = new RazasRepository(context);

            return View(repos.GetAll().Where(x=>x.Eliminado==0));
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
                    Repository<Paises> reposPaises = new Repository<Paises>(context);
                    vm.Paises = reposPaises.GetAll();
                    return View(vm);
                }
                else
                {
                    if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un archivo jpg menor a 2MB");
                        Repository<Paises> reposPaises = new Repository<Paises>(context);
                        vm.Paises = reposPaises.GetAll();
                        return View(vm);
                    }
                }
               
                RazasRepository repos = new RazasRepository(context);
                
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
            RazasRepository repos = new RazasRepository(context);
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
                RazasRepository repos = new RazasRepository(context);
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
                    original.IdPais = vm.Raza.IdPais;
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
                    repos.Update(original);
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
            RazasRepository repos = new RazasRepository(context);
            var razaBD = repos.GetById(id);
            if (razaBD != null)
                return  View(razaBD);
            else 
                return RedirectToAction("Index", "Home");
           
        }

        [HttpPost]
        public IActionResult Eliminar(Razas razatemp)
        {
            try
            {
                RazasRepository repos = new RazasRepository(context);

                var original = repos.GetById(razatemp.Id);
                original.Eliminado = 1;
                repos.Update(original);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View(razatemp);
            }            
        }
    }
}
