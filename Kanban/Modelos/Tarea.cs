using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Modelos
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaIncio { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Prioridad { get; set; }
        public int Monto { get; set; }
        public string Descripcion { get; set; }
        public string Resposable { get; set; }
        public string PAsociado { get; set; }

        //********************Relación**********************

        public int proyectoId { get; set; } //Se relaciona con el Id del proyecto
        [ForeignKey("proyectoId")]  //Instancia DataAnnotations
        public Proyecto Proyecto { get; set;} //Se llama a la entidad, modelo Proyecto


    }
}