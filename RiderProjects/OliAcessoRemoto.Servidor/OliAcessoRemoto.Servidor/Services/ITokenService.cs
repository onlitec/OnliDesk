namespace OliAcessoRemoto.Servidor.Services;

public interface ITokenService
{
    string GenerateToken(string clientId, string clientName);
    bool ValidateToken(string token);
    string? GetClientIdFromToken(string token);
}
