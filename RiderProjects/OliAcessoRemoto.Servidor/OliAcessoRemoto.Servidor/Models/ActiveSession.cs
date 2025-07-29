using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OliAcessoRemoto.Servidor.Models;

public class ActiveSession
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(11)]
    public string ControllerClientId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(11)]
    public string TargetClientId { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Ended, Failed
    
    // Navigation properties
    [ForeignKey(nameof(ControllerClientId))]
    public virtual Client? ControllerClient { get; set; }
    
    [ForeignKey(nameof(TargetClientId))]
    public virtual Client? TargetClient { get; set; }
}
