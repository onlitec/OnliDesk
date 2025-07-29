using System.ComponentModel.DataAnnotations;

namespace OliAcessoRemoto.Servidor.Models;

public class Client
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(11)]
    public string ClientId { get; set; } = string.Empty; // XXX XXX XXX format
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    
    public bool IsOnline { get; set; } = false;
    
    public string? ConnectionInfo { get; set; } // JSON with IP, version, etc.
    
    // Navigation properties
    public virtual ICollection<ActiveSession> ControllerSessions { get; set; } = new List<ActiveSession>();
    public virtual ICollection<ActiveSession> TargetSessions { get; set; } = new List<ActiveSession>();
    public virtual ICollection<ConnectionLog> ConnectionLogs { get; set; } = new List<ConnectionLog>();
}
