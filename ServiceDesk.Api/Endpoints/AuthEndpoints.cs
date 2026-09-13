using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.Api.Data;
using ServiceDesk.Api.Dtos;
using ServiceDesk.Api.Models;
using ServiceDesk.Api.Services;

namespace ServiceDesk.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Autenticación");

        // POST /auth/register - Registro de usuario con validación
        group.MapPost("/register", async (RegisterDto dto, IValidator<RegisterDto> validator, ServiceDeskDbContext db) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var emailExists = await db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                return Results.BadRequest(new { Message = "El correo electrónico ya se encuentra registrado." });
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok(new { Message = "Usuario registrado exitosamente.", user.Id, user.Email });
        });

        // POST /auth/login - Inicio de sesión y emisión de JWT
        group.MapPost("/login", async (LoginDto dto, ServiceDeskDbContext db, JwtService jwtService) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            var token = jwtService.GenerateToken(user);
            return Results.Ok(new { Token = token, user.Email, Role = user.Role.ToString() });
        });
    }
}