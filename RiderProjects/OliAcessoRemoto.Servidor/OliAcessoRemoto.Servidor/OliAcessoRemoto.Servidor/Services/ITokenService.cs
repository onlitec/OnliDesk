namespace OliAcessoRemoto.Servidor.Services;

public interface ITokenService
{
    string GenerateToken(string clientId, string clientName);
}
