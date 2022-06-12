using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kanban.Data;
using Kanban.Modelos;
using Kanban.Repository.IRepository;

namespace Kanban.Repository
{
    public class UsuarioRepository : IUsuarioRepository //Hereda la declaración de métodos de IUsuarioRepository y contiene instrucciones de cada uno
    {
        private readonly ApplicationDbContext _bd; //Instancia del contexto

        public UsuarioRepository(ApplicationDbContext bd)
        {
            _bd = bd; 
        }

        public bool ExisteUsuario(string email)
        {
            if (_bd.Usuario.Any(x=> x.Email == email))
            {
                return true;
            }
            return false;
        }

        public Usuario GetUsuario(int UsuarioId)
        {
            return _bd.Usuario.FirstOrDefault(c => c.Id == UsuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(c => c.Email).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }

        public Usuario Login(string email, string password)
        {
            var user = _bd.Usuario.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return null;
            }

            if(!VerificaPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        public Usuario Registro(Usuario usuario, string password)
        {
            byte[] passwordHash, passwordSalt;

            CrearPasswordHash(password, out passwordHash, out passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            _bd.Usuario.Add(usuario);
            Guardar();
            return usuario;
        }


        //Para REGISTRO
        private void CrearPasswordHash(string password,out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        //Para LOGIN
        private bool VerificaPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i<hashComputado.Length; i++)
                {
                    if (hashComputado[i] != passwordHash[i]) return false;
                   
                }
            }
            return true;
        }

    }
}
