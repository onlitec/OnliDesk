namespace OnliDesk.Server.Models.DTOs;

public class ClientRegistrationRequest
{
    public string Name { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
}

public class ClientRegistrationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class ClientStatusResponse
{
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
}

public class ConnectionRequest
{
    public string TargetClientId { get; set; } = string.Empty;
    public string RequestingClientId { get; set; } = string.Empty;
    public string? Password { get; set; }
}

public class ConnectionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
