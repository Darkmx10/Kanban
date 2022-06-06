using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Modelos.Dtos
{
    public class ProyectoDto
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage ="Campo requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime FechaInicio { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime FechaEntrega { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int Monto { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Empresa { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string EmpleadosA { get; set; }

    }
}