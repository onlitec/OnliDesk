using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace OliAcessoRemoto.Servidor.Services;

public class ClientIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("clientId")?.Value;
    }
}
