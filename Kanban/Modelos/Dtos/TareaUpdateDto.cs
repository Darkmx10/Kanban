using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Modelos.Dtos
{
    public class TareaUpdateDto
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime FechaInicio { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime FechaEntrega { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Prioridad { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int Monto { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Resposable { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string PAsociado { get; set; }

        //********************Relación**********************

        public int proyectoId { get; set; } //Se relaciona con el Id del proyecto

    }
}
