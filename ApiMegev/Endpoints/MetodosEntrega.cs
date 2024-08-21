using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class MetodosEntregaEndpoints
    {
        public static void RegistrarEndpointsMetodosEntrega(this IEndpointRouteBuilder rotas)
        {
            var rotaMetodosEntrega = rotas.MapGroup("/metodos-entrega");

            rotaMetodosEntrega.MapGet("/", async (MegevDbContext dbContext) =>
            {
                var metodosEntrega = await dbContext.MetodoEntrega.ToListAsync();
                return TypedResults.Ok(metodosEntrega);
            });

            rotaMetodosEntrega.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var metodoEntrega = await dbContext.MetodoEntrega.FindAsync(id);
                if (metodoEntrega == null)
                    return Results.NotFound();

                return TypedResults.Ok(metodoEntrega);
            }).Produces<MetodoEntrega>();

            rotaMetodosEntrega.MapPost("/", async (MegevDbContext dbContext, MetodoEntrega metodoEntrega) =>
            {
                dbContext.MetodoEntrega.Add(metodoEntrega);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/metodos-entrega/{metodoEntrega.Id}", metodoEntrega);
            });

            rotaMetodosEntrega.MapPut("/{id}", async (MegevDbContext dbContext, int id, MetodoEntrega metodoEntrega) =>
            {
                var metodoEntregaExistente = await dbContext.MetodoEntrega.FindAsync(id);
                if (metodoEntregaExistente == null)
                    return Results.NotFound();

                metodoEntrega.Id = id;
                dbContext.Entry(metodoEntregaExistente).CurrentValues.SetValues(metodoEntrega);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });

            rotaMetodosEntrega.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var metodoEntregaExistente = await dbContext.MetodoEntrega.FindAsync(id);
                if (metodoEntregaExistente == null)
                    return Results.NotFound();

                dbContext.MetodoEntrega.Remove(metodoEntregaExistente);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
        }
    }
}
