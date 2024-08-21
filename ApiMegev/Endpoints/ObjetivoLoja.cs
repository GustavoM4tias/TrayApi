using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace megev.Endpoints
{
    public static class ObjetivoLojaEndpoints
    {
        public static void RegistrarEndpointsObjetivoLoja(this IEndpointRouteBuilder rotas)
        {
            var rotaObjetivosLoja = rotas.MapGroup("/Objetivo-Loja");

            rotaObjetivosLoja.MapGet("/", async (MegevDbContext dbContext) =>
            {
                var objetivosLoja = await dbContext.ObjetivoLoja.ToListAsync();
                return TypedResults.Ok(objetivosLoja);
            });

            rotaObjetivosLoja.MapGet("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var objetivoLoja = await dbContext.ObjetivoLoja.FindAsync(id);
                if (objetivoLoja == null)
                    return Results.NotFound();

                return TypedResults.Ok(objetivoLoja);
            }).Produces<ObjetivoLoja>();

            rotaObjetivosLoja.MapPost("/", async (MegevDbContext dbContext, ObjetivoLoja objetivoLoja) =>
            {
                dbContext.ObjetivoLoja.Add(objetivoLoja);
                await dbContext.SaveChangesAsync();

                return TypedResults.Created($"/Objetivo-Loja/{objetivoLoja.Id}", objetivoLoja);
            });

            rotaObjetivosLoja.MapPut("/{id}", async (MegevDbContext dbContext, int id, ObjetivoLoja objetivoLoja) =>
            {
                var objetivoLojaExistente = await dbContext.ObjetivoLoja.FindAsync(id);
                if (objetivoLojaExistente == null)
                    return Results.NotFound();

                objetivoLoja.Id = id;
                dbContext.Entry(objetivoLojaExistente).CurrentValues.SetValues(objetivoLoja);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });

            rotaObjetivosLoja.MapDelete("/{id}", async (MegevDbContext dbContext, int id) =>
            {
                var objetivoLojaExistente = await dbContext.ObjetivoLoja.FindAsync(id);
                if (objetivoLojaExistente == null)
                    return Results.NotFound();

                dbContext.ObjetivoLoja.Remove(objetivoLojaExistente);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
        }
    }
}
