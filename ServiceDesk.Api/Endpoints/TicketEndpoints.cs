using System.Security.Claims;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.Api.Data;
using ServiceDesk.Api.Dtos;
using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tickets")
            .WithTags("Tickets")
            .RequireAuthorization();

        // GET /tickets - Listar tickets según el rol del usuario
        group.MapGet("/", async (ClaimsPrincipal user, ServiceDeskDbContext db) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = user.FindFirstValue(ClaimTypes.Role);

            var query = db.Tickets
                .Include(t => t.Client)
                .Include(t => t.Agent)
                .AsNoTracking();

            if (userRole == UserRole.Client.ToString())
            {
                query = query.Where(t => t.ClientId == userId);
            }

            var tickets = await query
                .Select(t => new TicketResponseDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status.ToString(),
                    t.Priority.ToString(),
                    t.CreatedAt,
                    t.ResolvedAt,
                    t.Client.Email,
                    t.Agent != null ? t.Agent.Email : null
                ))
                .ToListAsync();

            return Results.Ok(tickets);
        })
        .WithName("GetTickets")
        .WithSummary("Obtiene los tickets según el rol del usuario autenticado");

        // POST /tickets - Crear ticket con validación (Solo Clientes)
        group.MapPost("/", async (CreateTicketDto dto, IValidator<CreateTicketDto> validator, ClaimsPrincipal user, ServiceDeskDbContext db) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = TicketStatus.Open,
                ClientId = userId
            };

            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();

            return Results.Created($"/tickets/{ticket.Id}", new { ticket.Id, ticket.Title, ticket.Status });
        })
        .RequireAuthorization(policy => policy.RequireRole(UserRole.Client.ToString()))
        .WithName("CreateTicket")
        .WithSummary("Crea un nuevo ticket de soporte");

        // PUT /tickets/{id}/status - Cambiar estado y asignar agente (Agent o Admin)
        group.MapPut("/{id:int}/status", async (int id, UpdateTicketStatusDto dto, ServiceDeskDbContext db) =>
        {
            var ticket = await db.Tickets.FindAsync(id);
            if (ticket is null) 
                return Results.NotFound(new { Message = "El ticket solicitado no existe." });

            if (dto.AgentId.HasValue)
            {
                var agentExists = await db.Users.AnyAsync(u => u.Id == dto.AgentId.Value && 
                    (u.Role == UserRole.Agent || u.Role == UserRole.Admin));

                if (!agentExists) 
                    return Results.BadRequest(new { Message = "El ID asignado no pertenece a un Agente o Administrador válido." });

                ticket.AgentId = dto.AgentId.Value;
            }

            ticket.Status = dto.Status;

            if (dto.Status == TicketStatus.Resolved || dto.Status == TicketStatus.Closed)
            {
                ticket.ResolvedAt ??= DateTime.UtcNow;
            }
            else
            {
                ticket.ResolvedAt = null;
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .RequireAuthorization(policy => policy.RequireRole(UserRole.Agent.ToString(), UserRole.Admin.ToString()))
        .WithName("UpdateTicketStatus")
        .WithSummary("Actualiza el estado de un ticket y asigna un agente de soporte");
    }
}