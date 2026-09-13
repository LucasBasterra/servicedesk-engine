namespace ServiceDesk.Api.Models;

public enum UserRole
{
    Client = 1,      // Usuario final que reporta incidentes
    Agent = 2,       // Técnico de soporte que resuelve tickets
    Admin = 3        // Administrador del sistema
}

public enum TicketStatus
{
    Open = 1,        // Ticket recién creado
    InProgress = 2,  // Asignado a un agente y en atención
    Resolved = 3,    // Solución propuesta por el técnico
    Closed = 4       // Ticket verificado y cerrado
}

public enum TicketPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4     // Requiere atención inmediata por SLA
}