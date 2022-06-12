using Kanban.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Repository.IRepository
{
     public interface ITareaRepository  //Interface con declaración de métodos
   
    {
        ICollection<Tarea> GetTareas(); //Obtiene todas las tareas
        ICollection<Tarea> GetTareasEnProyectos(int PId); //Permite obtener las tareas en proyecto, todas las tareas que haya en un proyecto
        Tarea GetTarea(int TareaId);  //Obtiene tarea por ID

        //****Tipo bool para devolver verdadero o falso****
        bool ExisteTarea(String nombre); //Comprueba si existe por medio del nombre
        IEnumerable<Tarea> BuscarTarea(string nombre); //Opcion de filtrar o buscar tareas
        bool ExisteTarea(int id); //Comprueba si existe por medio del ID
        bool CrearTarea(Tarea tarea);
        bool ActualizarTarea(Tarea tarea);
        bool BorrarTarea(Tarea tarea);
        bool Guardar();

    }
}
