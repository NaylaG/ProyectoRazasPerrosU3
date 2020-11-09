using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ProyectoRazasPerrosU3.Models;
using ProyectoRazasPerrosU3.Models.ViewModels;
using ProyectoRazasPerrosU3.Repositories;

namespace ProyectoRazasPerrosU3.Controllers
{
    public class HomeController : Controller
    {
        sistem14_razasContext context;
      
        public HomeController(sistem14_razasContext ctx)
        {
            context = ctx;
         
        }

        [Route("ABC_RAZAS/{id}")]
        [Route("/index")]
        [Route("/")]
        public IActionResult Index(string id)
        {


            RazasRepository repos = new RazasRepository(context);
            IndexViewModel vm = new IndexViewModel
            {
                Razas = id == null ? repos.GetRazas() : repos.GetRazasByLetraInicial(id),
                LetrasIniciales = repos.GetLetrasIniciales().Distinct()
            };
            return View(vm);
        }

        [Route("Raza/{id}")]
        public IActionResult InfoPerro(string id)
        {
            RazasRepository repos = new RazasRepository(context);
            InfoPerroViewModel vm = new InfoPerroViewModel();
            vm.Raza = repos.GetRazaByNombre(id);

            if (vm.Raza == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                vm.OtrasRazas = repos.Get4RandomRazasExcept(id);
                return View(vm);
            }
        }
        [Route("/RazasPorPais")]
        public IActionResult RazasPorPais()
        {            
            RazaViewModel vm = new RazaViewModel();
            RazasRepository repos = new RazasRepository(context);
             //vm.Paises= context.Paises.Include(x=>x.Razas).ToList();           
            vm.Paises = repos.RazasPorPais();
            return View(vm);
        }
    }
}
