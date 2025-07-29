using System.ComponentModel.DataAnnotations;

namespace OliAcessoRemoto.Servidor.Models.DTOs;

public class ConnectionRequest
{
    [Required]
    [StringLength(11)]
    public string TargetClientId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(11)]
    public string RequesterClientId { get; set; } = string.Empty;
    
    public string? Password { get; set; }
}
