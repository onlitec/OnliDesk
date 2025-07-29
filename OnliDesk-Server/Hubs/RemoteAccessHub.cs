using Microsoft.AspNetCore.SignalR;
using OnliDesk.Server.Services;

namespace OnliDesk.Server.Hubs;

public class RemoteAccessHub : Hub
{
    private readonly IClientService _clientService;
    private readonly ILogger<RemoteAccessHub> _logger;

    public RemoteAccessHub(IClientService clientService, ILogger<RemoteAccessHub> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var clientId = Context.GetHttpContext()?.Request.Query["clientId"].ToString();
        
        if (!string.IsNullOrEmpty(clientId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"client_{clientId}");
            await _clientService.UpdateClientActivityAsync(clientId);
            _logger.LogInformation("Client {ClientId} connected to SignalR hub", clientId);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var clientId = Context.GetHttpContext()?.Request.Query["clientId"].ToString();
        
        if (!string.IsNullOrEmpty(clientId))
        {
            await _clientService.SetClientOfflineAsync(clientId);
            _logger.LogInformation("Client {ClientId} disconnected from SignalR hub", clientId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinClientGroup(string clientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"client_{clientId}");
        await _clientService.UpdateClientActivityAsync(clientId);
    }

    public async Task LeaveClientGroup(string clientId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"client_{clientId}");
    }

    public async Task SendConnectionRequest(string targetClientId, string requestingClientId, string message)
    {
        await Clients.Group($"client_{targetClientId}")
            .SendAsync("ConnectionRequest", requestingClientId, message);
        
        _logger.LogInformation("Connection request sent from {RequestingClientId} to {TargetClientId}", 
                              requestingClientId, targetClientId);
    }

    public async Task SendConnectionResponse(string requestingClientId, bool accepted, string message)
    {
        await Clients.Group($"client_{requestingClientId}")
            .SendAsync("ConnectionResponse", accepted, message);
        
        _logger.LogInformation("Connection response sent to {RequestingClientId}: {Accepted}", 
                              requestingClientId, accepted);
    }

    public async Task SendScreenData(string targetClientId, string screenData)
    {
        await Clients.Group($"client_{targetClientId}")
            .SendAsync("ScreenData", screenData);
    }

    public async Task SendInputEvent(string targetClientId, string eventType, object eventData)
    {
        await Clients.Group($"client_{targetClientId}")
            .SendAsync("InputEvent", eventType, eventData);
    }

    public async Task UpdateHeartbeat(string clientId)
    {
        await _clientService.UpdateClientActivityAsync(clientId);
    }
}
