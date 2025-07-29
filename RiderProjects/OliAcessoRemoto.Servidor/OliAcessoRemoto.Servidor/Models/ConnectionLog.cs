using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OliAcessoRemoto.Servidor.Models;

public class ConnectionLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(11)]
    public string ClientId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Details { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(ClientId))]
    public virtual Client? Client { get; set; }
}
