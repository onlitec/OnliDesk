using OliAcessoRemoto.Servidor.Models.DTOs;

namespace OliAcessoRemoto.Servidor.Services;

public interface IClientService
{
    Task<string> GenerateUniqueClientId();
    Task<ClientRegistrationResponse> RegisterClientAsync(ClientRegistrationRequest request);
}
