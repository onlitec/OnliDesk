namespace OliAcessoRemoto.Servidor.Models.DTOs;

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
