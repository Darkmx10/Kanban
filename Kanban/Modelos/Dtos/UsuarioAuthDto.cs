using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Modelos.Dtos
{
    public class UsuarioAuthDto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Apellidos { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(16, MinimumLength =8, ErrorMessage ="La contraseña debe contener entre 8 y 16 caracteres")]
        public string Password { get; set; }
    }
}
