using System.Security.Claims;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.Api.Data;
using ServiceDesk.Api.Dtos;
using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tickets/{ticketId:int}/comments")
            .WithTags("Comentarios")
            .RequireAuthorization();

        // GET /tickets/{ticketId}/comments - Historial de comentarios
        group.MapGet("/", async (int ticketId, ClaimsPrincipal user, ServiceDeskDbContext db) =>
        {
            var role = user.FindFirstValue(ClaimTypes.Role);

            var query = db.Comments
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .AsNoTracking();

            // Clientes no ven notas internas de soporte
            if (role == UserRole.Client.ToString())
            {
                query = query.Where(c => !c.IsInternal);
            }

            var comments = await query.Select(c => new
            {
                c.Id,
                c.Content,
                c.IsInternal,
                c.CreatedAt,
                Author = c.User.FullName,
                Role = c.User.Role.ToString()
            }).ToListAsync();

            return Results.Ok(comments);
        });

        // POST /tickets/{ticketId}/comments - Agregar comentario con validación
        group.MapPost("/", async (int ticketId, CreateCommentDto dto, IValidator<CreateCommentDto> validator, ClaimsPrincipal user, ServiceDeskDbContext db) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = user.FindFirstValue(ClaimTypes.Role);

            var ticketExists = await db.Tickets.AnyAsync(t => t.Id == ticketId);
            if (!ticketExists) 
                return Results.NotFound(new { Message = "El ticket especificado no existe." });

            bool isInternal = dto.IsInternal && (role == UserRole.Agent.ToString() || role == UserRole.Admin.ToString());

            var comment = new TicketComment
            {
                TicketId = ticketId,
                UserId = userId,
                Content = dto.Content,
                IsInternal = isInternal
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            return Results.Created($"/tickets/{ticketId}/comments/{comment.Id}", new { comment.Id, comment.Content, comment.IsInternal });
        });
    }
}