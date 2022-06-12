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
    [Authorize] //Con este comando el usuario se debe loggear para manipular todas las tareas
    [Route("api/Tareas")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiKanbanTareas")] //Multiple documentación (Startup)
    public class TareasController : Controller
    {
        private readonly ITareaRepository _taRepo;
        private readonly IMapper _mapper;

       public TareasController(ITareaRepository taRepo, IMapper mapper)
        {
            _taRepo = taRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtener todas las tareas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TareaDto>))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(400, Type = typeof(List<TareaDto>))] //400: Bad request
        public IActionResult GetTarea() //Obtiene todos los tareas haciendo uso de Dto para no exponer el modelo
        {
            var listaTareas = _taRepo.GetTareas();
            var listaTareasDto = new List<TareaDto>(); //Instancia de Tarea Dto
           
            foreach (var lista in listaTareas)
            {
                listaTareasDto.Add(_mapper.Map <TareaDto>(lista));
            }
            return Ok(listaTareasDto);
        }

        /// <summary>
        /// Obtener una tarea por ID
        /// </summary>
        /// <param name="tareaId"></param>
        /// <returns></returns>
        [HttpGet("{tareaId:int}", Name = "GetTarea")] //Obtiene tarea por ID
        [ProducesResponseType(200, Type = typeof(TareaDto))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(404)] //No encontrado
        [ProducesDefaultResponseType] //Error
        public IActionResult GetTarea(int tareaId)
        {
            var itemTarea = _taRepo.GetTarea(tareaId);

            if (itemTarea == null)
            {
                return NotFound();
            }                             //Se le pasa el Dto el item para que lo busque
            var itemTareaDto = _mapper.Map<TareaDto>(itemTarea);
            return Ok(itemTareaDto);
        }

        /// <summary>
        /// Buscar las tareas de un proyecto recibiendo Id de proyecto
        /// </summary>
        /// <param name="proyectoId"></param>
        /// <returns></returns>
        [HttpGet("GetTareaEnProyecto/{proyectoId:int}")]
        [ProducesResponseType(200, Type = typeof(TareaDto))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(404)] 
        [ProducesDefaultResponseType] 
        public IActionResult GetTareaEnProyecto(int proyectoId) {  //Metodo que permite buscar las tareas de un proyecto recibiendo Id de proyecto
            var listaTarea = _taRepo.GetTareasEnProyectos(proyectoId); //Accede al repositorio, accede al metodo GetTareasEnProyecto y le pasa el Id que se recibe para que lo busque y queden guardadas en listaTarea
           
            if (listaTarea == null) //Condicional para validar si lo que se valida es nulo
            {
                return NotFound();
            }

            var itemTarea = new List<TareaDto>(); //
            foreach (var item in listaTarea)
            { //Recorre toda la lista y guarda los Item de un en uno
                itemTarea.Add(_mapper.Map<TareaDto>(item));
            }
            return Ok(itemTarea);
        }

        /// <summary>
        /// Buscar tarea por nombre
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        [HttpGet("Buscar")]
        [ProducesResponseType(200, Type = typeof(TareaDto))] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(404)] //No encontrado
        [ProducesDefaultResponseType] //Error
        public IActionResult Buscar(string nombre) //Buscar tarea por nombre
        {
            try
            {
                var resultado = _taRepo.BuscarTarea(nombre);
                if (resultado.Any())
                {
                    return Ok(resultado); //Si encuentra un resultado lo retorna
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error, recuperando datos de base de datos");
            }
        }

        /// <summary>
        /// Crear nueva tarea
        /// </summary>
        /// <param name="TareaDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TareaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearTarea([FromForm] TareaCreateDto TareaDto) //FromForm: Los datos que se obtengan en el cuerpo del formulario tendra una peticion vinculada a los datos de TareaDto
        {
            if (TareaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_taRepo.ExisteTarea(TareaDto.Nombre)) //Valida si existe un Tarea con el mismo nombre
            {
                ModelState.AddModelError("", "El tarea ya existe");
                return StatusCode(404, ModelState);
            }

            //---------------------------------------------------------
            
                        //Vincula y recibe el tareaDto y lo pasa a Tarea
            var tarea = _mapper.Map<Tarea>(TareaDto); //Accede a las tareas

            if (!_taRepo.CrearTarea(tarea))
            {
                ModelState.AddModelError("", $"Error al guardar el registro{tarea.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTarea", new { tareaId = tarea.Id}, tarea); //Devuelve ultimo registro en el Body de postman

        }


        /// <summary>
        /// Actualizar tarea
        /// </summary>
        /// <param name="tareaId"></param>
        /// <param name="tareaDto"></param>
        /// <returns></returns>
        [HttpPatch("{tareaId:int}", Name = "ActualizarTarea")]
        [ProducesResponseType(204)] //Corrige el codigo de respuesta a 204
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarTarea(int tareaId, [FromBody]TareaDto tareaDto)
        {
            if (tareaDto == null || tareaId != tareaDto.Id) //Valida que el tarea exista
            {
                return BadRequest(ModelState);
            }

            var tarea = _mapper.Map<Tarea>(tareaDto); //Si pasa la validación, se asigan los nuevos valores

            if (!_taRepo.ActualizarTarea(tarea))
            {
                ModelState.AddModelError("", $"Error al actualizar el registro{tarea.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        /// <summary>
        /// Borrar tarea
        /// </summary>
        /// <param name="tareaId"></param>
        /// <returns></returns>
        [HttpDelete("{tareaId:int}", Name = "BorrarTarea")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] //Corrige el codigo de respuesta a 200
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarTarea(int tareaId)
        {
            if (!_taRepo.ExisteTarea(tareaId)) //Si no existe el tarea retorna un NotFound
            {
                return NotFound();
            }

            var tarea = _taRepo.GetTarea(tareaId); //Si pasa la validación anterior, busca el tarea por ID


            if (!_taRepo.BorrarTarea(tarea))
            {
                ModelState.AddModelError("", $"Error al borrar el registro{tarea.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }


    }
}
