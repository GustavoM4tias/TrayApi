using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class LojaEndpoints
    {
        public static void RegistrarEndpointsLoja(this IEndpointRouteBuilder rotas)
        {
            var rotaLojas = rotas.MapGroup("/lojas");

            // Get all Lojas
            rotaLojas.MapGet("/", async (MegevDbContext dbContext) =>
            {
                var lojas = await dbContext.Loja.ToListAsync();
                return TypedResults.Ok(lojas);
            }).Produces<IEnumerable<Loja>>(StatusCodes.Status200OK);

            // Get Loja by Id
            rotaLojas.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var loja = await dbContext.Loja.FindAsync(id);
                if (loja == null)
                    return Results.NotFound();

                return TypedResults.Ok(loja);
            }).Produces<Loja>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

            // Create new Loja
            rotaLojas.MapPost("/", async (MegevDbContext dbContext, Loja loja) =>
            {
                dbContext.Loja.Add(loja);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/lojas/{loja.Id}", loja);
            }).Produces<Loja>(StatusCodes.Status201Created);

            // Update existing Loja
            rotaLojas.MapPut("/{id}", async (MegevDbContext dbContext, int id, Loja loja) =>
            {
                var lojaExistente = await dbContext.Loja.FindAsync(id);
                if (lojaExistente == null)
                    return Results.NotFound();

                loja.Id = id;
                dbContext.Entry(lojaExistente).CurrentValues.SetValues(loja);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            }).Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

            // Delete existing Loja
            rotaLojas.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var lojaExistente = await dbContext.Loja.FindAsync(id);
                if (lojaExistente == null)
                    return Results.NotFound();

                dbContext.Loja.Remove(lojaExistente);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            }).Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
        }
    }
}
