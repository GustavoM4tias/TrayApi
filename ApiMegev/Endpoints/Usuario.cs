using megev.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace megev.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static void RegistrarEndpointsUsuarios(this IEndpointRouteBuilder rotas)
        {
            var rotaUsuarios = rotas.MapGroup("/usuarios");

            // Endpoint de Registro
            rotaUsuarios.MapPost("/registrar", async (MegevDbContext dbContext, UsuarioDTO usuarioDto) =>
            {
                var usuarioExistente = await dbContext.Usuario.SingleOrDefaultAsync(u => u.Email == usuarioDto.Email);
                if (usuarioExistente != null)
                {
                    return Results.BadRequest("Email já cadastrado.");
                }

                var usuario = new Usuario(
                    usuarioDto.Nome,
                    usuarioDto.Sobrenome,
                    usuarioDto.Email,
                    BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha),
                    usuarioDto.SaldoConta
                );

                dbContext.Usuario.Add(usuario);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Usuário registrado com sucesso.");
            });

            // Endpoint de Login
            rotaUsuarios.MapPost("/login", async (MegevDbContext dbContext, UsuarioLoginDTO loginDto, IConfiguration config) =>
            {
                var usuario = await dbContext.Usuario.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.Senha))
                {
                    return Results.Unauthorized();
                }

                // Criar o token JWT
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                };

                var jwtSettings = config.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new
                {
                    Mensagem = "Login bem-sucedido.",
                    UsuarioId = usuario.Id,
                    Token = tokenString
                });
            });

            // Endpoint de Logout (não faz nada com JWT, mas pode ser usado para frontend)
            rotaUsuarios.MapPost("/logout", () =>
            {
                return Results.Ok("Logout realizado com sucesso.");
            });

            // Endpoint para Obter Dados do Usuário Logado
            rotaUsuarios.MapGet("/me", async (HttpContext context, MegevDbContext dbContext) =>
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized(); // Retorna 401 se não estiver autenticado
                }

                var usuario = await dbContext.Usuario.SingleOrDefaultAsync(u => u.Id.ToString() == userId);

                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
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
            }).RequireAuthorization();

            // Endpoint para Obter Dados de Usuário por ID
            rotaUsuarios.MapGet("/{id}", async (int id, MegevDbContext dbContext) =>
            {
                var usuario = await dbContext.Usuario.SingleOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
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
            });

            // Endpoint para Atualizar Dados de um Usuário (PUT)
            rotaUsuarios.MapPut("/{id}", async (int id, MegevDbContext dbContext, UsuarioDTO usuarioDto) =>
            {
                var usuario = await dbContext.Usuario.SingleOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }

                usuario.Nome = usuarioDto.Nome;
                usuario.Sobrenome = usuarioDto.Sobrenome;
                usuario.Email = usuarioDto.Email;
                if (!string.IsNullOrEmpty(usuarioDto.Senha))
                {
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);
                }
                usuario.SaldoConta = usuarioDto.SaldoConta;

                dbContext.Usuario.Update(usuario);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Usuário atualizado com sucesso.");
            }).RequireAuthorization();

            // Endpoint para Deletar um Usuário (DELETE)
            rotaUsuarios.MapDelete("/{id}", async (int id, MegevDbContext dbContext) =>
            {
                var usuario = await dbContext.Usuario.SingleOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }

                dbContext.Usuario.Remove(usuario);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Usuário deletado com sucesso.");
            }).RequireAuthorization();
        }
    }
}
