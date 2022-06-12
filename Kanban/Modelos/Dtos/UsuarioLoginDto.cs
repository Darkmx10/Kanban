using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Modelos.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Password { get; set; }
    }
}
