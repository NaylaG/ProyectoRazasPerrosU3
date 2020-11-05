using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoRazasPerrosU3.Models.ViewModels
{
    public class RazasPaisViewModel
    {
        public IEnumerable<Paises> Paises { get; set; }
        public IEnumerable<Razas> Razas { get; set; }
    }
}
