using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class CategoriaProdutoEndpoints
    {
        public static void RegistrarEndpointsCategoriaProduto(this IEndpointRouteBuilder rotas)
        {
            var rotaCategoriasProduto = rotas.MapGroup("/categorias-produto");

            rotaCategoriasProduto.MapGet("/", async (MegevDbContext dbContext) =>
            {
                var categoriasProduto = await dbContext.CategoriaProduto.ToListAsync();
                return TypedResults.Ok(categoriasProduto);
            });

            rotaCategoriasProduto.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var categoriaProduto = await dbContext.CategoriaProduto.FindAsync(id);
                if (categoriaProduto == null)
                    return Results.NotFound();

                return TypedResults.Ok(categoriaProduto);
            }).Produces<CategoriaProduto>();

            rotaCategoriasProduto.MapPost("/", async (MegevDbContext dbContext, CategoriaProduto categoriaProduto) =>
            {
                dbContext.CategoriaProduto.Add(categoriaProduto);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/categorias-produto/{categoriaProduto.Id}", categoriaProduto);
            });

            rotaCategoriasProduto.MapPut("/{id}", async (MegevDbContext dbContext, int id, CategoriaProduto categoriaProduto) =>
            {
                var categoriaProdutoExistente = await dbContext.CategoriaProduto.FindAsync(id);
                if (categoriaProdutoExistente == null)
                    return Results.NotFound();

                categoriaProduto.Id = id;
                dbContext.Entry(categoriaProdutoExistente).CurrentValues.SetValues(categoriaProduto);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });

            rotaCategoriasProduto.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var categoriaProdutoExistente = await dbContext.CategoriaProduto.FindAsync(id);
                if (categoriaProdutoExistente == null)
                    return Results.NotFound();

                dbContext.CategoriaProduto.Remove(categoriaProdutoExistente);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
        }
    }
}
