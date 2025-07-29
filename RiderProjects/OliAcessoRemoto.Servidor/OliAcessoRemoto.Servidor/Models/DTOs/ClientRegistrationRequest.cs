using System.ComponentModel.DataAnnotations;

namespace OliAcessoRemoto.Servidor.Models.DTOs;

public class ClientRegistrationRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public string? ConnectionInfo { get; set; }
}
