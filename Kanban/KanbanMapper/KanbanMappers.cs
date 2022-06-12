using AutoMapper;
using Kanban.Modelos;
using Kanban.Modelos.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.KanbanMapper
{              
    public class KanbanMappers : Profile //Vincula el modelo Dto con el modelo Proyecto
    {
        public KanbanMappers()
        {
            CreateMap<Proyecto, ProyectoDto>().ReverseMap(); //Reverse map: Viseversa 
            CreateMap<Tarea, TareaDto>().ReverseMap(); //Reverse map: Viseversa 
            CreateMap<Tarea, TareaCreateDto>().ReverseMap(); //Reverse map: Viseversa 
            CreateMap<Tarea, TareaUpdateDto>().ReverseMap(); //Reverse map: Viseversa 
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
        }
    }
}
