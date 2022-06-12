using Kanban.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Repository.IRepository
{
     public interface IUsuarioRepository  //Interface con declaración de métodos
   
    {
        ICollection<Usuario> GetUsuarios(); //Obtiene todas los usuarios
       
        Usuario GetUsuario(int UsuarioId);  //Obtiene usuario por ID

        //****Tipo bool para devolver verdadero o falso****
        bool ExisteUsuario(string email); //Comprueba si existe por medio del Email
       
        Usuario Registro(Usuario email, string password);

        Usuario Login(string email, string password);

        bool Guardar();
    }
}
