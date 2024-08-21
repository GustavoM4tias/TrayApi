using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class ProdutoEndpoints
    {
        public static void RegistrarEndpointsProdutos(this IEndpointRouteBuilder rotas)
        {
            var rotaProdutos = rotas.MapGroup("/produtos");

            rotaProdutos.MapGet("/", async (MegevDbContext dbContext, int page = 1, int limit = 12) =>
            {
                if (page < 1 || limit < 1)
                {
                    return Results.BadRequest("O número da página e o limite devem ser maiores que zero.");
                }

                var totalProdutos = await dbContext.Produto.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProdutos / (double)limit);

                if (page > totalPages)
                {
                    return Results.BadRequest("Número da página maior que o total de páginas disponível.");
                }

                var produtos = await dbContext.Produto
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var paginatedResult = new
                {
                    Total = totalProdutos,
                    PageSize = limit,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    Produtos = produtos
                };

                return Results.Ok(paginatedResult);
            });

            rotaProdutos.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var produto = await dbContext.Produto.FindAsync(id);
                if (produto == null)
                    return Results.NotFound();

                return Results.Ok(produto);
            }).Produces<Produto>();

            rotaProdutos.MapPost("/", async (MegevDbContext dbContext, Produto produto) =>
            {
                dbContext.Produto.Add(produto);
                await dbContext.SaveChangesAsync();

                return Results.Created($"/produtos/{produto.Id}", produto);
            });

            rotaProdutos.MapPut("/{id}", async (MegevDbContext dbContext, int id, Produto produto) =>
            {
                var produtoExistente = await dbContext.Produto.FindAsync(id);
                if (produtoExistente == null)
                    return Results.NotFound();

                produto.Id = id;
                dbContext.Entry(produtoExistente).CurrentValues.SetValues(produto);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });

            rotaProdutos.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var produtoExistente = await dbContext.Produto.FindAsync(id);
                if (produtoExistente == null)
                    return Results.NotFound();

                dbContext.Produto.Remove(produtoExistente);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
