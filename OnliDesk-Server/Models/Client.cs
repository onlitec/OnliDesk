namespace OnliDesk.Server.Models;

public class Client
{
    public int Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public bool IsOnline { get; set; } = true;
}

public class ActiveSession
{
    public int Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
}

public class ConnectionLog
{
    public int Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string? TargetClientId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public string? Details { get; set; }
}
