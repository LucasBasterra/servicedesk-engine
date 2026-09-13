namespace ServiceDesk.Api.Models;

public class TicketComment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsInternal { get; set; } // Notas privadas visibles solo para Agentes/Admin

    // Foreign Keys y Propiedades de Navegación
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}