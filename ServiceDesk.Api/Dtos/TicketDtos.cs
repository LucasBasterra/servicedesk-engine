using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Dtos;


public record CreateCommentDto(string Content, bool IsInternal = false);


// Creación de ticket por parte del cliente
public record CreateTicketDto(
    string Title,
    string Description,
    TicketPriority Priority
);

// Actualización de estado o asignación por parte del soporte/admin
public record UpdateTicketStatusDto(
    TicketStatus Status,
    int? AgentId
);

// Respuesta estructurada del ticket
public record TicketResponseDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime CreatedAt,
    DateTime? ResolvedAt,
    string ClientEmail,
    string? AgentEmail
);