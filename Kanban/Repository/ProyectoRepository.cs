using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kanban.Data;
using Kanban.Modelos;
using Kanban.Repository.IRepository;

namespace Kanban.Repository
{
    public class ProyectoRepository : IProyectoRepository //Hereda la declaración de métodos de IProyectoRepository y contiene instrucciones de cada uno
    {
        private readonly ApplicationDbContext _bd; //Instancia del contexto

        public ProyectoRepository(ApplicationDbContext bd) //Instancia acceso a la base de datos
        {
            _bd = bd; //Para usar en todos los metodos
        }

        public bool ActualizarProyecto(Proyecto proyecto)
        {
            _bd.Proyecto.Update(proyecto);
            return Guardar();
        }

        public bool BorrarProyecto(Proyecto proyecto)
        {
            _bd.Proyecto.Remove(proyecto);
            return Guardar();
        }

        public bool CrearProyecto(Proyecto proyecto)
        {
            _bd.Proyecto.Add(proyecto);
            return Guardar();
        }

        public bool ExisteProyecto(string nombre)
        {       //Busca si existe un nombre igual(Convertido a minuscula y quitando espacios en blanco) al nombre que se esta pasando por parametros
            bool valor = _bd.Proyecto.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteProyecto(int id)
        {
            return _bd.Proyecto.Any(c => c.Id == id);
        }

        public Proyecto GetProyecto(int ProyectoId)
        {   //Busca el proyecto por medio del ID que se recibe por parametro
            return _bd.Proyecto.FirstOrDefault(c => c.Id == ProyectoId);
        }

        public ICollection<Proyecto> GetProyectos()
        {   //Devuelve todos los proyetos existentes
            return _bd.Proyecto.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {   // Guarda los cambios con True cuando sea mayor igual a cero, si no, devuelve false
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
