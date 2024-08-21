using megev;
using megev.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace megev
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Cria��o da WebApplication
            var builder = WebApplication.CreateBuilder(args);

            // Configura��o do Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configura��o CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyMethod()
                           .AllowAnyOrigin()
                           .AllowAnyHeader();
                });
            });

            // Configura��o do DbContext
            builder.Services.AddDbContext<MegevDbContext>(options =>
            {
                // Configure aqui a sua string de conex�o ou leia de vari�veis de ambiente
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                                 ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
            });

            // Constru��o da WebApplication
            var app = builder.Build();

            // Middleware de desenvolvimento para Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Registro dos endpoints da API
            app.RegistrarEndpointsUsuarios();
            app.RegistrarEndpointsProdutos();
            app.RegistrarEndpointsMetodoPagamento();
            app.RegistrarEndpointsMetodosEntrega();
            app.RegistrarEndpointsObjetivoLoja();
            app.RegistrarEndpointsLoja();
            app.RegistrarEndpointsCategoriaProduto();

            // Habilita CORS
            app.UseCors();

            // Execu��o da aplica��o
            app.Run();
        }
    }
}
