using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Modelos
{
    public class Proyecto
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int Monto { get; set; }
        public string Descripcion { get; set; }
        public string Empresa { get; set; }
        public string EmpleadosA { get; set; }

    }
}