namespace ServiceDesk.Api.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Foreign Keys y Propiedades de Navegación
    public int ClientId { get; set; }
    public User Client { get; set; } = null!;

    public int? AgentId { get; set; }
    public User? Agent { get; set; }

    // Historial y seguimiento
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}