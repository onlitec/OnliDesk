using OliAcessoRemoto.Servidor.Models;
using OliAcessoRemoto.Servidor.Models.DTOs;

namespace OliAcessoRemoto.Servidor.Services;

public interface IClientService
{
    Task<string> GenerateUniqueClientId();
    Task<ClientRegistrationResponse> RegisterClientAsync(ClientRegistrationRequest request);
    Task<ClientStatusResponse?> GetClientStatusAsync(string clientId);
    Task<List<OnlineClientDto>> GetOnlineClientsAsync();
    Task<bool> ValidateClientAsync(string clientId);
    Task LogConnectionActivity(string clientId, string action, string details);
    Task<ConnectionResponse> RequestConnectionAsync(ConnectionRequest request);
    Task CleanupExpiredSessions();
}
