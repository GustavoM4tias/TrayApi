using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static void RegistrarEndpointsUsuarios(this IEndpointRouteBuilder rotas)
        {
            var rotaUsuarios = rotas.MapGroup("/usuarios");

            rotaUsuarios.MapGet("/", async (MegevDbContext dbContext, string? nome, string? sobrenome) =>
            {
                IQueryable<Usuario> usuariosFiltrados = dbContext.Usuario;

                if (!string.IsNullOrEmpty(nome))
                {
                    usuariosFiltrados = usuariosFiltrados.Where(u => u.Nome.Contains(nome));
                }

                if (!string.IsNullOrEmpty(sobrenome))
                {
                    usuariosFiltrados = usuariosFiltrados.Where(u => u.Sobrenome.Contains(sobrenome));
                }

                var usuariosDto = await usuariosFiltrados
                    .Select(u => new UsuarioOutputDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Sobrenome = u.Sobrenome,
                        Email = u.Email,
                        SaldoConta = u.SaldoConta
                    })
                    .ToListAsync();

                return TypedResults.Ok(usuariosDto);
            });

            rotaUsuarios.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var usuario = await dbContext.Usuario.FindAsync(id);
                if (usuario is null)
                {
                    return Results.NotFound();
                }

                var usuarioDto = new UsuarioOutputDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Sobrenome = usuario.Sobrenome,
                    Email = usuario.Email,
                    SaldoConta = usuario.SaldoConta
                };

                return TypedResults.Ok(usuarioDto);
            }).Produces<UsuarioOutputDto>();

            rotaUsuarios.MapPost("/", async (MegevDbContext dbContext, UsuarioInputDto usuarioDto) =>
            {
                var usuario = new Usuario(
                    usuarioDto.Nome,
                    usuarioDto.Sobrenome,
                    usuarioDto.Email,
                    usuarioDto.Senha,
                    usuarioDto.SaldoConta
                );

                dbContext.Usuario.Add(usuario);
                await dbContext.SaveChangesAsync();

                var usuarioOutputDto = new UsuarioOutputDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Sobrenome = usuario.Sobrenome,
                    Email = usuario.Email,
                    SaldoConta = usuario.SaldoConta
                };

                return TypedResults.Created($"/usuarios/{usuario.Id}", usuarioOutputDto);
            });

            rotaUsuarios.MapPut("/{id}", async (MegevDbContext dbContext, int id, UsuarioInputDto usuarioDto) =>
            {
                var usuarioEncontrado = await dbContext.Usuario.FindAsync(id);
                if (usuarioEncontrado is null)
                {
                    return Results.NotFound();
                }

                usuarioEncontrado.Nome = usuarioDto.Nome;
                usuarioEncontrado.Sobrenome = usuarioDto.Sobrenome;
                usuarioEncontrado.Email = usuarioDto.Email;
                usuarioEncontrado.Senha = usuarioDto.Senha;
                usuarioEncontrado.SaldoConta = usuarioDto.SaldoConta;

                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });

            rotaUsuarios.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var usuarioEncontrado = await dbContext.Usuario.FindAsync(id);
                if (usuarioEncontrado is null)
                {
                    return Results.NotFound();
                }

                dbContext.Usuario.Remove(usuarioEncontrado);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
        }
    }
}
