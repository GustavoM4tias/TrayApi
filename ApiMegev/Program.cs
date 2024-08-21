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
            // Criação da WebApplication
            var builder = WebApplication.CreateBuilder(args);

            // Configuração do Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configuração CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyMethod()
                           .AllowAnyOrigin()
                           .AllowAnyHeader();
                });
            });

            // Configuração do DbContext
            builder.Services.AddDbContext<MegevDbContext>(options =>
            {
                // Configure aqui a sua string de conexão ou leia de variáveis de ambiente
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                                 ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
            });

            // Construção da WebApplication
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

            // Execução da aplicação
            app.Run();
        }
    }
}
