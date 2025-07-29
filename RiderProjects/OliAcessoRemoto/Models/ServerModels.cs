using System.ComponentModel.DataAnnotations;

namespace OliAcessoRemoto.Models;

// DTOs para comunicação com o servidor
public class ClientRegistrationRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public string? ConnectionInfo { get; set; }
}

public class ClientRegistrationResponse
{
    public bool Success { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? Message { get; set; }
}

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

public class ConnectionResponse
{
    public bool Success { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string? Message { get; set; }
}

public class ClientStatusResponse
{
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class OnlineClientDto
{
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
}

public class InputEventDto
{
    public string EventType { get; set; } = string.Empty; // Mouse, Keyboard
    public string Action { get; set; } = string.Empty; // Click, Move, KeyDown, KeyUp
    public int X { get; set; }
    public int Y { get; set; }
    public string? Key { get; set; }
    public bool CtrlPressed { get; set; }
    public bool ShiftPressed { get; set; }
    public bool AltPressed { get; set; }
}
