namespace OliAcessoRemoto.Servidor.Models.DTOs;

public class ConnectionResponse
{
    public bool Success { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string? Message { get; set; }
}
