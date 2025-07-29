namespace OliAcessoRemoto.Servidor.Models.DTOs;

public class OnlineClientDto
{
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
}
