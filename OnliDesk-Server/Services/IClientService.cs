using OnliDesk.Server.Models;
using OnliDesk.Server.Models.DTOs;

namespace OnliDesk.Server.Services;

public interface IClientService
{
    Task<ClientRegistrationResponse> RegisterClientAsync(ClientRegistrationRequest request, string ipAddress);
    Task<ClientStatusResponse?> GetClientStatusAsync(string clientId);
    Task<List<ClientStatusResponse>> GetOnlineClientsAsync();
    Task<ConnectionResponse> RequestConnectionAsync(ConnectionRequest request, string ipAddress);
    Task UpdateClientActivityAsync(string clientId);
    Task SetClientOfflineAsync(string clientId);
}

public interface ITokenService
{
    string GenerateToken(string clientId);
    bool ValidateToken(string token, out string clientId);
}
