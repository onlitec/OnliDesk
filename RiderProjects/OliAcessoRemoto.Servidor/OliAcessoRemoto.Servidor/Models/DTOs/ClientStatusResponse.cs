namespace OliAcessoRemoto.Servidor.Models.DTOs;

public class ClientStatusResponse
{
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public string Name { get; set; } = string.Empty;
}
