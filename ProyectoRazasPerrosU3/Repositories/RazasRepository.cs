using Microsoft.EntityFrameworkCore;
using ProyectoRazasPerrosU3.Models;
using ProyectoRazasPerrosU3.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ProyectoRazasPerrosU3.Repositories
{

    public class RazasRepository: Repository<Razas>
    {
        
        public RazasRepository(sistem14_razasContext ctx):base(ctx)
        {

        }
        public IEnumerable<RazaViewModel> GetRazas()
        {
            return Context.Razas.OrderBy(x => x.Nombre).Where(x => x.Eliminado==0)
                .Select(x => new RazaViewModel
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                });
        }
        public override IEnumerable<Razas> GetAll()
        {
            return base.GetAll().OrderBy(x=>x.Nombre);
        }

        public IEnumerable<RazaViewModel> GetRazasByLetraInicial(string letra)
        {
            return GetRazas().Where(x => x.Nombre.StartsWith(letra));
        }


        public IEnumerable<char> GetLetrasIniciales()
        {
        
            return Context.Razas
             .OrderBy(x => x.Nombre)
             .Select(x => x.Nombre.ToUpper().First());
        }

        public Razas GetRazaByNombre(string nombre)
        {
            nombre = nombre.Replace("-", " ");
            return Context.Razas.Include(x=>x.Estadisticasraza)
                .Include(x => x.Caracteristicasfisicas)
                .Include(x => x.IdPaisNavigation)
                .FirstOrDefault(x => x.Nombre == nombre);
        }

        public override Razas GetById(object id)
        {
            return Context.Razas.Include(x => x.IdPaisNavigation).Include(x=>x.Estadisticasraza).Include(x=>x.Caracteristicasfisicas).FirstOrDefault(x => x.Id == (uint)id);
        }

        public IEnumerable<RazaViewModel> Get4RandomRazasExcept(string nombre)
        {
            nombre = nombre.Replace("-", " ");
            Random r = new Random();
            
            return Context.Razas
                .Where(x => x.Nombre != nombre)
                .ToList()
                .OrderBy(x => r.Next())
                .Take(4)
                .Select(x => new RazaViewModel { Id = x.Id, Nombre = x.Nombre });
        }

        public override bool Validate(Razas entidad)
        {
            if(entidad.Id<=0)
            { throw new Exception("Debe ingresar el ID de la raza"); }
            if (string.IsNullOrEmpty(entidad.Nombre))
            { throw new Exception("Debe ingresar el nombre de la raza"); }
            if (string.IsNullOrEmpty(entidad.OtrosNombres))
            { throw new Exception("Debe indicar si existen otros nombres de la raza"); }
            if (string.IsNullOrEmpty(entidad.Descripcion))
            { throw new Exception("Debe ingresar la descripcion de la raza"); }
            //if (Context.Paises.Any(x=>x.Id==entidad.IdPais))
            //{ throw new Exception("Debe seleccionar un pais de origen de la raza valido"); }

            if(entidad==null ||  entidad.PesoMax<=0)
            { throw new Exception("Debe ingresar un peso maximo valido para la raza"); }
            if(entidad == null || entidad.AlturaMax <= 0)
            { throw new Exception("Debe ingresar una altura maxima valido para la raza"); }
            if (entidad == null || entidad.PesoMin <= 0)
            { throw new Exception("Debe ingresar un peso minimo valido para la raza"); }
            if (entidad == null || entidad.AlturaMin <= 0)
            { throw new Exception("Debe ingresar una altura minima valido para la raza"); }
            if (entidad == null || entidad.EsperanzaVida <= 0)
            { throw new Exception("Debe ingresar una esperanza de vida valida para la raza"); }

            if (entidad.Estadisticasraza.NivelEnergia< 0 || entidad.Estadisticasraza.NivelEnergia>10)
            { throw new Exception("Debe ingresar un nivel de energia en un rango de 0 a 10"); }
            if (entidad.Estadisticasraza.FacilidadEntrenamiento <  0 || entidad.Estadisticasraza.FacilidadEntrenamiento > 10)
            { throw new Exception("Debe ingresar un nivel de entendimiento en un rango de 0 a 10"); }
            if (entidad.Estadisticasraza.AmistadDesconocidos < 0 || entidad.Estadisticasraza.AmistadDesconocidos > 10)
            { throw new Exception("Debe ingresar un nivel de amistad con desconocidos en un rango de 0 a 10"); }
            if (entidad.Estadisticasraza.AmistadPerros < 0 || entidad.Estadisticasraza.AmistadPerros > 10)
            { throw new Exception("Debe ingresar un nivel de amistad con perros en un rango de 0 a 10"); }
            if (entidad.Estadisticasraza.NecesidadCepillado < 0 || entidad.Estadisticasraza.NecesidadCepillado > 10)
            { throw new Exception("Debe ingresar un nivel de nesecidad de cepillado en un rango de 0 a 10"); }


            if (string.IsNullOrEmpty(entidad.Caracteristicasfisicas.Patas))
            { throw new Exception("Debe ingresar las caracteristicas de las patas de la raza"); }
            if (string.IsNullOrEmpty(entidad.Caracteristicasfisicas.Cola))
            { throw new Exception("Debe ingresar las caracteristicas de la cola de la raza"); }
            if (string.IsNullOrEmpty(entidad.Caracteristicasfisicas.Hocico))
            { throw new Exception("Debe ingresar las caracteristicas del hocico de la raza"); }
            if (string.IsNullOrEmpty(entidad.Caracteristicasfisicas.Pelo))
            { throw new Exception("Debe ingresar las caracteristicas del pelo de la raza"); }
            if (string.IsNullOrEmpty(entidad.Caracteristicasfisicas.Color))
            { throw new Exception("Debe ingresar las caracteristicas del color de la raza"); }


            if (Context.Razas.Any(x => x.Nombre == entidad.Nombre && x.Id != entidad.Id))
            {
                throw new Exception("Ya existe una raza registrada con el mismo nombre");
            }

            if (!Context.Paises.Any(x => x.Id == entidad.IdPais))
            {
                throw new Exception("No existe el pais especificado");
            }







            return true;
        }


    }
}

