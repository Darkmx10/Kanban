using AutoMapper;
using Kanban.Data;
using Kanban.Helpers;
using Kanban.KanbanMapper;
using Kanban.Repository;
using Kanban.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kanban
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   //Config de dependencia
            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddScoped<IProyectoRepository, ProyectoRepository>(); //Uso global de los Repository
            services.AddScoped<ITareaRepository, TareaRepository>(); //Uso global de los Repository
            services.AddScoped<IUsuarioRepository, UsuarioRepository>(); //Uso global de los Repository

            //Dependencia de token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAutoMapper(typeof(KanbanMappers));

            //Config de documentación
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ApiKanban", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Kanban",
                    Version = "1",
                    Description = "Backend Kanban",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "josuepjmx@gmail.com",
                        Name = "Josué Pérez",
                        Url = new Uri("https://darkenergy.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                options.SwaggerDoc("ApiKanbanProyectos", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Proyectos",
                    Version = "1",
                    Description = "Backend Kanban",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "josuepjmx@gmail.com",
                        Name = "Josué Pérez",
                        Url = new Uri("https://darkenergy.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                options.SwaggerDoc("ApiKanbanTareas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Tareas",
                    Version = "1",
                    Description = "Backend Kanban",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "josuepjmx@gmail.com",
                        Name = "Josué Pérez",
                        Url = new Uri("https://darkenergy.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                options.SwaggerDoc("ApiKanbanUsuarios", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Usuarios",
                    Version = "1",
                    Description = "Backend Kanban",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "josuepjmx@gmail.com",
                        Name = "Josué Pérez",
                        Url = new Uri("https://darkenergy.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                //Habilita los comentarios creados en los controladores para que sean mostrados en la documentacion
                var archivoXmlComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXmlComentarios);
                options.IncludeXmlComments(rutaApiComentarios);

                //Autenticación en documentación
                //Se define el esquema de seguridad
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticación JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id="Bearer",
                            Type= ReferenceType.SecurityScheme
                        }
                    }, new List<string>()
                }
                });
           
            });

                services.AddControllers();

                //Soporte para CORS
                services.AddCors();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else  //Manejo de excepciones globales
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            app.UseHttpsRedirection();

            //Linea para documentacion api
            app.UseSwagger();
            app.UseSwaggerUI(options => 
            {
                options.SwaggerEndpoint("/swagger/ApiKanbanProyectos/swagger.json", "API Proyectos");   //SwaggerEndpoint: Permite acceder a la documentacion por una ruta mas simple
                options.SwaggerEndpoint("/swagger/ApiKanbanTareas/swagger.json", "API Tareas");
                options.SwaggerEndpoint("/swagger/ApiKanbanUsuarios/swagger.json", "API Usuarios");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            // Autenticación y autorización
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Soporte para CORS: Usar la api desde un dominio diferente
            //Permite usar la api desde cualquier origen, cualquier metodo y cualquier encabezdpo
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

         
        }
    }
}
