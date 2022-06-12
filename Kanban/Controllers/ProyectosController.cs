using AutoMapper;
using Kanban.Modelos;
using Kanban.Modelos.Dtos;
using Kanban.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Controllers
{

    [Authorize] //Con este comando el usuario se debe loggear para manipular todos los proyectos
    [Route("api/Proyectos")]
    [ApiController]
    [ApiExplorerSettings(GroupName= "ApiKanbanProyectos")] //Multiple documentación (Startup)
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class ProyectosController : Controller
    {
        private readonly IProyectoRepository _prRepo;
        private readonly IMapper _mapper;

       public ProyectosController(IProyectoRepository prRepo, IMapper mapper)
        {
            _prRepo = prRepo;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Obtener todos los proyectos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<ProyectoDto>))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(400, Type = typeof(List<ProyectoDto>))] //400: Bad request
        public IActionResult GetProyecto() //Obtiene todos los proyectos haciendo uso de Dto para no exponer el modelo
        {
            var listaProyectos = _prRepo.GetProyectos();
            var listaProyectosDto = new List<ProyectoDto>(); //Instancia de Proyecto Dto
           
            foreach (var lista in listaProyectos)
            {
                listaProyectosDto.Add(_mapper.Map <ProyectoDto>(lista));
            }
            return Ok(listaProyectosDto);
        }

        /// <summary>
        ///Obtener un proyecto individual
        /// </summary>
        /// <param name="proyectoId">Este es el ID del Proyecto</param>
        /// <returns></returns>
        [HttpGet("{proyectoId:int}", Name = "GetProyecto")] //Obtiene proyecto por ID. Se le especifica que el metodo es GetProyecto
        [ProducesResponseType(200, Type = typeof(ProyectoDto))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(404)] //No encontrado
        [ProducesDefaultResponseType] //Error
        public IActionResult GetProyecto(int proyectoId)
        {
            var itemProyecto = _prRepo.GetProyecto(proyectoId);

            if (itemProyecto == null)
            {
                return NotFound();
            }                             //Se le pasa el Dto el item para que lo busque
            var itemProyectoDto = _mapper.Map<ProyectoDto>(itemProyecto);
            return Ok(itemProyectoDto);
        }


        /// <summary>
        /// Crear nuevo proyecto
        /// </summary>
        /// <param name="proyectoDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ProyectoDto))] 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearProyecto([FromBody] ProyectoDto proyectoDto) //FromBody: Lo que se obtenga en el cuerpo de la petición estará vinculado a los datos de ProyectoDto
        {
            if (proyectoDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_prRepo.ExisteProyecto(proyectoDto.Nombre)) //Valida si existe un Proyecto con el mismo nombre
            {
                ModelState.AddModelError("", "El proyecto ya existe");
                return StatusCode(404, ModelState);
            }
                        //Vincula y recibe el proyectoDto y lo pasa a Proyecto
            var proyecto = _mapper.Map<Proyecto>(proyectoDto);

            if (!_prRepo.CrearProyecto(proyecto))
            {
                ModelState.AddModelError("", $"Error al guardar el registro{proyecto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetProyecto", new { proyectoId = proyecto.Id}, proyecto); //Devuelve ultimo registro en el Body de postman
        }

        /// <summary>
        /// Actualizar un proyeto existente
        /// </summary>
        /// <param name="proyectoId"></param>
        /// <param name="proyectoDto"></param>
        /// <returns></returns>
        [HttpPatch("{proyectoId:int}", Name = "ActualizarProyecto")]
        [ProducesResponseType(204)] //Corrige el codigo de respuesta a 204
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarProyecto(int proyectoId, [FromBody]ProyectoDto proyectoDto)
        {
            if (proyectoDto == null || proyectoId != proyectoDto.Id) //Valida que el proyecto exista
            {
                return BadRequest(ModelState);
            }

            var proyecto = _mapper.Map<Proyecto>(proyectoDto); //Si pasa la validación, se asigan los nuevos valores

            if (!_prRepo.ActualizarProyecto(proyecto))
            {
                ModelState.AddModelError("", $"Error al actualizar el registro{proyecto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        /// <summary>
        /// Borrar un proyecto existente
        /// </summary>
        /// <param name="proyectoId"></param>
        /// <returns></returns>
        [HttpDelete("{proyectoId:int}", Name = "BorrarProyecto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarProyecto(int proyectoId)
        {
            if (!_prRepo.ExisteProyecto(proyectoId)) //Si no existe el proyecto retorna un NotFound
            {
                return NotFound();
            }

            var proyecto = _prRepo.GetProyecto(proyectoId); //Si pasa la validación anterior, busca el proyecto por ID

            if (!_prRepo.BorrarProyecto(proyecto))
            {
                ModelState.AddModelError("", $"Error al borrar el registro{proyecto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
