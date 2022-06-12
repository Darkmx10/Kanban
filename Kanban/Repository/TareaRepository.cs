using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kanban.Data;
using Kanban.Modelos;
using Kanban.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Repository
{
    public class TareaRepository : ITareaRepository //Hereda la declaración de métodos de IProyectoRepository y contiene instrucciones de cada uno
    {
        private readonly ApplicationDbContext _bd; //Instancia del contexto

        public TareaRepository(ApplicationDbContext bd) //Instancia acceso a la base de datos
        {
            _bd = bd; //Para usar en todos los metodos
        }

        public bool ActualizarTarea(Tarea tarea)
        {
            _bd.Tarea.Update(tarea);
            return Guardar();
        }

        public bool BorrarTarea(Tarea tarea)
        {
            _bd.Tarea.Remove(tarea);
            return Guardar();
        }

        public IEnumerable<Tarea> BuscarTarea(string nombre)
        {
            IQueryable<Tarea> query = _bd.Tarea; //IQueryable: se crea variable de nombre query que permite hacer consultas a la BD sobre 

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre)); //Si encuentra coincidencias lo guarda en Query
            }
            return query.ToList();
        }

        public bool CrearTarea(Tarea tarea)
        {
            _bd.Tarea.Add(tarea);
            return Guardar();
        }

        public bool ExisteTarea(string nombre)
        {
            bool valor = _bd.Proyecto.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteTarea(int id)
        {
            return _bd.Tarea.Any(c => c.Id == id);
        }

        public Tarea GetTarea(int TareaId)  //Obtiene tarea individual
        {
            return _bd.Tarea.FirstOrDefault(p => p.Id == TareaId);
        }

        public ICollection<Tarea> GetTareas()
        {  //Accede a tareas, lo ordena y lo convierte en una lista para obtener toda la lista de tareas
            return _bd.Tarea.OrderBy(c=> c.Nombre).ToList();
        }

        public ICollection<Tarea> GetTareasEnProyectos(int PId)
        {
   //Accede a tareas, por medio de Include permite incluir otra tabla de Proyeto(pr), y se valida por medio del Where que pr(Proyecto) cuando accede por Id sea igual a PId(el parámetro que recibe) y manda todas las tareas por medio de ToList
            return _bd.Tarea.Include(pr => pr.Proyecto).Where(pr => pr.proyectoId == PId).ToList();
        }

        public bool Guardar()
        {
            // Guarda los cambios con True cuando sea mayor igual a cero, si no, devuelve false
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
