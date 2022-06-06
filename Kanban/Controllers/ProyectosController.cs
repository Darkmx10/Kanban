using AutoMapper;
using Kanban.Modelos;
using Kanban.Modelos.Dtos;
using Kanban.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Controllers
{
    [Route("api/Proyectos")]
    [ApiController]
    public class ProyectosController : Controller
    {
        private readonly IProyectoRepository _prRepo;
        private readonly IMapper _mapper;

       public ProyectosController(IProyectoRepository prRepo, IMapper mapper)
        {
            _prRepo = prRepo;
            _mapper = mapper;
        }

        [HttpGet]
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

        //Se le especifica que el metodo es GetProyecto
        [HttpGet("{proyectoId:int}", Name = "GetProyecto")] //Obtiene proyecto por ID
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

        [HttpPost]
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

        [HttpPatch("{proyectoId:int}", Name = "ActualizarProyecto")]
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


        [HttpDelete("{proyectoId:int}", Name = "BorrarProyecto")]
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
