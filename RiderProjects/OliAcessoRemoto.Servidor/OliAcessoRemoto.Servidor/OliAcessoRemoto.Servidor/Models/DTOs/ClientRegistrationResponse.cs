namespace OliAcessoRemoto.Servidor.Models.DTOs;

public class ClientRegistrationResponse
{
    public bool Success { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? Message { get; set; }
}
