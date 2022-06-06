using Kanban.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Repository.IRepository
{
     public interface IProyectoRepository  //Interface con declaración de métodos
   
    {
        ICollection<Proyecto> GetProyectos(); //Obtiene todos los proyectos
        Proyecto GetProyecto(int ProyectoId);  //Obtiene proyecto por ID
        bool ExisteProyecto(String nombre); //Comprueba si existe por medio del nombre

        bool ExisteProyecto(int id); //Comprueba si existe por medio del ID
        bool CrearProyecto(Proyecto proyecto);
        bool ActualizarProyecto(Proyecto proyecto);
        bool BorrarProyecto(Proyecto proyecto);
        bool Guardar();

    }
}
