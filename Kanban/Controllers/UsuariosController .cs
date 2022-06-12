using AutoMapper;
using Kanban.Modelos;
using Kanban.Modelos.Dtos;
using Kanban.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("api/Usuarios")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiKanbanUsuarios")] //Multiple documentación (Startup)
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

       public UsuariosController(IUsuarioRepository userRepo, IMapper mapper, IConfiguration config)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// Obtener todos los usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<UsuarioDto>))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(400, Type = typeof(List<UsuarioDto>))] //400: Bad request
        public IActionResult GetUsuarios() //Obtiene todos los usuarios haciendo uso de Dto para no exponer el modelo
        {
            var listaUsuarios = _userRepo.GetUsuarios();
            var listaUsuariosDto = new List<UsuarioDto>(); //Instancia de Usuario Dto
           
            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map <UsuarioDto>(lista));
            }
            return Ok(listaUsuariosDto);
        }

        /// <summary>
        /// Obtener un usuario individual por su ID
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        [HttpGet("{usuarioId:int}", Name = "GetUsuario")] //Obtiene usuario por ID
        [ProducesResponseType(200, Type = typeof(UsuarioDto))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(404)] //No encontrado
        [ProducesDefaultResponseType] //Error
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _userRepo.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }                             //Se le pasa el Dto el item para que lo busque
            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);
            return Ok(itemUsuarioDto);
        }

        /// <summary>
        /// Registrar usuario
        /// </summary>
        /// <param name="usuarioAuthDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(201, Type = typeof(UsuarioDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Registro(UsuarioAuthDto usuarioAuthDto)
        {
            usuarioAuthDto.Email = usuarioAuthDto.Email.ToLower();

            if (_userRepo.ExisteUsuario(usuarioAuthDto.Email))
            {
                return BadRequest("Este correo ya se encuentra registrado");
            }

            var usuarioACrear = new Usuario
            {
                Email = usuarioAuthDto.Email,
                Nombre = usuarioAuthDto.Nombre,
                Apellidos = usuarioAuthDto.Apellidos
                 
            };

            var usuarioCreado = _userRepo.Registro(usuarioACrear, usuarioAuthDto.Password);
            return Ok(usuarioCreado);
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="usuarioLoginDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType] //Error
        public IActionResult Login(UsuarioLoginDto usuarioLoginDto)
        {
             
                var usuarioDesdeRepo = _userRepo.Login(usuarioLoginDto.Email, usuarioLoginDto.Password);

                if (usuarioDesdeRepo == null)
                {
                    return Unauthorized();
                }

                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuarioDesdeRepo.Id.ToString()),
            new Claim(ClaimTypes.Name, usuarioDesdeRepo.Email.ToString())
            };

                //Generando TOKEN
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //crea las credenciales del token

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1), //El login genera un token para ingresar a zonas protegidas de la appp. Este token debe tener un tiempo de expiración. Para este caso 1 día
                    SigningCredentials = credenciales
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });
        }
    }
}
