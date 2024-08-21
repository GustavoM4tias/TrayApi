using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class MetodoPagamentoEndpoints
    {
        public static void RegistrarEndpointsMetodoPagamento(this IEndpointRouteBuilder rotas)
        {
            var rotaMetodosPagamento = rotas.MapGroup("/metodos-pagamento");

            rotaMetodosPagamento.MapGet("/", async (MegevDbContext dbContext) =>
            {
                var metodosPagamento = await dbContext.MetodoPagamento.ToListAsync();
                return TypedResults.Ok(metodosPagamento);
            });

            rotaMetodosPagamento.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var metodoPagamento = await dbContext.MetodoPagamento.FindAsync(id);
                if (metodoPagamento == null)
                    return Results.NotFound();

                return TypedResults.Ok(metodoPagamento);
            }).Produces<MetodoPagamento>();

            rotaMetodosPagamento.MapPost("/", async (MegevDbContext dbContext, MetodoPagamento metodoPagamento) =>
            {
                dbContext.MetodoPagamento.Add(metodoPagamento);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/metodos-pagamento/{metodoPagamento.Id}", metodoPagamento);
            });

            rotaMetodosPagamento.MapPut("/{id}", async (MegevDbContext dbContext, int id, MetodoPagamento metodoPagamento) =>
            {
                var metodoPagamentoExistente = await dbContext.MetodoPagamento.FindAsync(id);
                if (metodoPagamentoExistente == null)
                    return Results.NotFound();

                metodoPagamento.Id = id;
                dbContext.Entry(metodoPagamentoExistente).CurrentValues.SetValues(metodoPagamento);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });

            rotaMetodosPagamento.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var metodoPagamentoExistente = await dbContext.MetodoPagamento.FindAsync(id);
                if (metodoPagamentoExistente == null)
                    return Results.NotFound();

                dbContext.MetodoPagamento.Remove(metodoPagamentoExistente);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
        }
    }
}
