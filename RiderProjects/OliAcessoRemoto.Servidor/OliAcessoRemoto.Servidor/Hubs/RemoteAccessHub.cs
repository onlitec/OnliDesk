using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OliAcessoRemoto.Servidor.Data;
using OliAcessoRemoto.Servidor.Models;
using OliAcessoRemoto.Servidor.Models.DTOs;
using OliAcessoRemoto.Servidor.Services;

namespace OliAcessoRemoto.Servidor.Hubs;

public class RemoteAccessHub : Hub
{
    private readonly RemoteAccessDbContext _context;
    private readonly IClientService _clientService;
    private readonly ILogger<RemoteAccessHub> _logger;

    public RemoteAccessHub(RemoteAccessDbContext context, IClientService clientService, ILogger<RemoteAccessHub> logger)
    {
        _context = context;
        _clientService = clientService;
        _logger = logger;
    }

    public async Task RegisterClient(string clientInfo)
    {
        try
        {
            var clientId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(clientId))
            {
                await Clients.Caller.SendAsync("RegistrationFailed", "Invalid client identifier");
                return;
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client != null)
            {
                client.IsOnline = true;
                client.LastSeen = DateTime.UtcNow;
                client.ConnectionInfo = clientInfo;
                
                await _context.SaveChangesAsync();
                
                // Add to SignalR group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Client_{clientId}");
                
                await Clients.Caller.SendAsync("RegistrationSuccess", clientId);
                _logger.LogInformation("Client {ClientId} registered successfully", clientId);
            }
            else
            {
                await Clients.Caller.SendAsync("RegistrationFailed", "Client not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering client");
            await Clients.Caller.SendAsync("RegistrationFailed", "Registration failed");
        }
    }

    public async Task RequestConnection(string targetId, string requesterId)
    {
        try
        {
            var targetClient = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == targetId && c.IsOnline);
            var requesterClient = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == requesterId && c.IsOnline);

            if (targetClient == null)
            {
                await Clients.Caller.SendAsync("ConnectionRequestFailed", "Target client not found or offline");
                return;
            }

            if (requesterClient == null)
            {
                await Clients.Caller.SendAsync("ConnectionRequestFailed", "Requester client not found or offline");
                return;
            }

            // Send connection request to target client
            await Clients.Group($"Client_{targetId}").SendAsync("IncomingConnectionRequest", requesterId, requesterClient.Name);
            
            // Log the connection request
            await _clientService.LogConnectionActivity(requesterId, "ConnectionRequested", $"Requested connection to {targetId}");
            
            _logger.LogInformation("Connection request from {RequesterId} to {TargetId}", requesterId, targetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing connection request");
            await Clients.Caller.SendAsync("ConnectionRequestFailed", "Connection request failed");
        }
    }

    public async Task RespondToConnection(string requesterId, bool approved)
    {
        try
        {
            var currentClientId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(currentClientId))
            {
                return;
            }

            if (approved)
            {
                // Create active session
                var session = new ActiveSession
                {
                    ControllerClientId = requesterId,
                    TargetClientId = currentClientId,
                    StartTime = DateTime.UtcNow,
                    Status = "Active"
                };

                _context.ActiveSessions.Add(session);
                await _context.SaveChangesAsync();

                // Notify both clients
                await Clients.Group($"Client_{requesterId}").SendAsync("ConnectionApproved", currentClientId, session.Id.ToString());
                await Clients.Caller.SendAsync("ConnectionEstablished", requesterId, session.Id.ToString());
                
                // Log the approval
                await _clientService.LogConnectionActivity(currentClientId, "ConnectionApproved", $"Approved connection from {requesterId}");
                await _clientService.LogConnectionActivity(requesterId, "ConnectionEstablished", $"Connection established with {currentClientId}");
            }
            else
            {
                // Notify requester of denial
                await Clients.Group($"Client_{requesterId}").SendAsync("ConnectionDenied", currentClientId);
                
                // Log the denial
                await _clientService.LogConnectionActivity(currentClientId, "ConnectionDenied", $"Denied connection from {requesterId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to connection");
        }
    }

    public async Task SendScreenData(string targetId, byte[] screenData)
    {
        try
        {
            // Forward screen data to target client
            await Clients.Group($"Client_{targetId}").SendAsync("ReceiveScreenData", Context.UserIdentifier, screenData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending screen data");
        }
    }

    public async Task SendInputEvent(string targetId, InputEventDto inputEvent)
    {
        try
        {
            // Forward input event to target client
            await Clients.Group($"Client_{targetId}").SendAsync("ReceiveInputEvent", Context.UserIdentifier, inputEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending input event");
        }
    }

    public override async Task OnConnectedAsync()
    {
        var clientId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(clientId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Client_{clientId}");
            _logger.LogInformation("Client {ClientId} connected", clientId);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var clientId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(clientId))
        {
            // Update client status to offline
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client != null)
            {
                client.IsOnline = false;
                client.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            // End any active sessions
            var activeSessions = await _context.ActiveSessions
                .Where(s => (s.ControllerClientId == clientId || s.TargetClientId == clientId) && s.Status == "Active")
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.Status = "Ended";
                session.EndTime = DateTime.UtcNow;
            }

            if (activeSessions.Any())
            {
                await _context.SaveChangesAsync();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Client_{clientId}");
            _logger.LogInformation("Client {ClientId} disconnected", clientId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
