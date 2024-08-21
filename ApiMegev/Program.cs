using megev.Endpoints;
using megev;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configuração CORS (Permitir que qualquer site use a API)
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()   // Permite qualquer origem
                      .AllowAnyMethod()   // Permite qualquer método (GET, POST, etc.)
                      .AllowAnyHeader();  // Permite qualquer cabeçalho
            });
        });

        // Configuração do DbContext
        builder.Services.AddDbContext<MegevDbContext>(options =>
        {
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                             ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
        });

        // Configuração do JWT
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ClockSkew = TimeSpan.Zero // Sem atraso na expiração do token
                };
            });

        // Configuração da Autorização
        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(); // Aplicar a configuração de CORS
        app.UseAuthentication();
        app.UseAuthorization();

        // Registro dos endpoints da API
        app.RegistrarEndpointsUsuarios();
        app.RegistrarEndpointsProdutos();
        app.RegistrarEndpointsMetodoPagamento();
        app.RegistrarEndpointsMetodosEntrega();
        app.RegistrarEndpointsObjetivoLoja();
        app.RegistrarEndpointsLoja();
        app.RegistrarEndpointsCategoriaProduto();

        app.Run();
    }
}
