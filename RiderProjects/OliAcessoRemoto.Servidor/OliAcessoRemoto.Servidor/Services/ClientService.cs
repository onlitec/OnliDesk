using Microsoft.EntityFrameworkCore;
using OliAcessoRemoto.Servidor.Data;
using OliAcessoRemoto.Servidor.Models;
using OliAcessoRemoto.Servidor.Models.DTOs;

namespace OliAcessoRemoto.Servidor.Services;

public class ClientService : IClientService
{
    private readonly RemoteAccessDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ClientService> _logger;

    public ClientService(RemoteAccessDbContext context, ITokenService tokenService, ILogger<ClientService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<string> GenerateUniqueClientId()
    {
        string clientId;
        bool exists;
        
        do
        {
            // Generate 9-digit ID in XXX XXX XXX format
            var random = new Random();
            var part1 = random.Next(100, 999);
            var part2 = random.Next(100, 999);
            var part3 = random.Next(100, 999);
            
            clientId = $"{part1} {part2} {part3}";
            exists = await _context.Clients.AnyAsync(c => c.ClientId == clientId);
        } 
        while (exists);

        return clientId;
    }

    public async Task<ClientRegistrationResponse> RegisterClientAsync(ClientRegistrationRequest request)
    {
        try
        {
            var clientId = await GenerateUniqueClientId();
            
            var client = new Client
            {
                ClientId = clientId,
                Name = request.Name,
                ConnectionInfo = request.ConnectionInfo,
                IsOnline = true,
                LastSeen = DateTime.UtcNow
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(clientId, request.Name);

            await LogConnectionActivity(clientId, "ClientRegistered", $"Client {request.Name} registered");

            _logger.LogInformation("Client registered: {ClientId} - {Name}", clientId, request.Name);

            return new ClientRegistrationResponse
            {
                Success = true,
                ClientId = clientId,
                Token = token,
                Message = "Registration successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering client: {Name}", request.Name);
            return new ClientRegistrationResponse
            {
                Success = false,
                Message = "Registration failed"
            };
        }
    }

    public async Task<ClientStatusResponse?> GetClientStatusAsync(string clientId)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
        
        if (client == null)
            return null;

        return new ClientStatusResponse
        {
            IsOnline = client.IsOnline,
            LastSeen = client.LastSeen,
            Name = client.Name
        };
    }

    public async Task<List<OnlineClientDto>> GetOnlineClientsAsync()
    {
        return await _context.Clients
            .Where(c => c.IsOnline)
            .Select(c => new OnlineClientDto
            {
                ClientId = c.ClientId,
                Name = c.Name,
                LastSeen = c.LastSeen
            })
            .ToListAsync();
    }

    public async Task<bool> ValidateClientAsync(string clientId)
    {
        return await _context.Clients.AnyAsync(c => c.ClientId == clientId && c.IsOnline);
    }

    public async Task LogConnectionActivity(string clientId, string action, string details)
    {
        try
        {
            var log = new ConnectionLog
            {
                ClientId = clientId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.ConnectionLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging connection activity for client {ClientId}", clientId);
        }
    }

    public async Task<ConnectionResponse> RequestConnectionAsync(ConnectionRequest request)
    {
        try
        {
            var targetClient = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == request.TargetClientId && c.IsOnline);
            var requesterClient = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == request.RequesterClientId && c.IsOnline);

            if (targetClient == null)
            {
                return new ConnectionResponse
                {
                    Success = false,
                    Message = "Target client not found or offline"
                };
            }

            if (requesterClient == null)
            {
                return new ConnectionResponse
                {
                    Success = false,
                    Message = "Requester client not found or offline"
                };
            }

            // Check if there's already an active session
            var existingSession = await _context.ActiveSessions
                .FirstOrDefaultAsync(s => s.ControllerClientId == request.RequesterClientId && 
                                         s.TargetClientId == request.TargetClientId && 
                                         s.Status == "Active");

            if (existingSession != null)
            {
                return new ConnectionResponse
                {
                    Success = false,
                    Message = "Connection already exists"
                };
            }

            await LogConnectionActivity(request.RequesterClientId, "ConnectionRequested", $"Requested connection to {request.TargetClientId}");

            return new ConnectionResponse
            {
                Success = true,
                Message = "Connection request sent"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing connection request");
            return new ConnectionResponse
            {
                Success = false,
                Message = "Connection request failed"
            };
        }
    }

    public async Task CleanupExpiredSessions()
    {
        try
        {
            var expiredTime = DateTime.UtcNow.AddMinutes(-30); // 30 minutes timeout
            
            var expiredSessions = await _context.ActiveSessions
                .Where(s => s.Status == "Active" && s.StartTime < expiredTime)
                .ToListAsync();

            foreach (var session in expiredSessions)
            {
                session.Status = "Expired";
                session.EndTime = DateTime.UtcNow;
            }

            if (expiredSessions.Any())
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} expired sessions", expiredSessions.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired sessions");
        }
    }
}
