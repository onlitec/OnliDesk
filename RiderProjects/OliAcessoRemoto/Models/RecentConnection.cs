namespace OliAcessoRemoto.Models;

public class RecentConnection
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime LastConnection { get; set; }
    public bool IsOnline { get; set; }
    
    public string LastConnectionFormatted => LastConnection.ToString("dd/MM/yyyy HH:mm");
}
