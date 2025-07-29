using Microsoft.EntityFrameworkCore;
using OnliDesk.Server.Data;
using OnliDesk.Server.Models;
using OnliDesk.Server.Models.DTOs;

namespace OnliDesk.Server.Services;

public class ClientService : IClientService
{
    private readonly OnliDeskDbContext _context;
    private readonly ClientIdProvider _idProvider;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ClientService> _logger;

    public ClientService(OnliDeskDbContext context, ClientIdProvider idProvider, 
                        ITokenService tokenService, ILogger<ClientService> logger)
    {
        _context = context;
        _idProvider = idProvider;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<ClientRegistrationResponse> RegisterClientAsync(ClientRegistrationRequest request, string ipAddress)
    {
        try
        {
            var clientId = _idProvider.GenerateClientId();
            
            var client = new Client
            {
                ClientId = clientId,
                Name = request.Name,
                ComputerName = request.ComputerName,
                UserName = request.UserName,
                IpAddress = ipAddress,
                Version = request.Version,
                CreatedAt = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                IsOnline = true
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(clientId);

            _logger.LogInformation("Client registered: {ClientId} - {Name}", clientId, request.Name);

            return new ClientRegistrationResponse
            {
                Success = true,
                Message = "Client registered successfully",
                ClientId = clientId,
                Token = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering client");
            return new ClientRegistrationResponse
            {
                Success = false,
                Message = "Registration failed"
            };
        }
    }

    public async Task<ClientStatusResponse?> GetClientStatusAsync(string clientId)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

        if (client == null) return null;

        return new ClientStatusResponse
        {
            ClientId = client.ClientId,
            Name = client.Name,
            IsOnline = client.IsOnline,
            LastSeen = client.LastSeen
        };
    }

    public async Task<List<ClientStatusResponse>> GetOnlineClientsAsync()
    {
        return await _context.Clients
            .Where(c => c.IsOnline)
            .Select(c => new ClientStatusResponse
            {
                ClientId = c.ClientId,
                Name = c.Name,
                IsOnline = c.IsOnline,
                LastSeen = c.LastSeen
            })
            .ToListAsync();
    }

    public async Task<ConnectionResponse> RequestConnectionAsync(ConnectionRequest request, string ipAddress)
    {
        try
        {
            var targetClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == request.TargetClientId);

            if (targetClient == null || !targetClient.IsOnline)
            {
                return new ConnectionResponse
                {
                    Success = false,
                    Message = "Target client not found or offline"
                };
            }

            // Log connection attempt
            var log = new ConnectionLog
            {
                ClientId = request.RequestingClientId,
                TargetClientId = request.TargetClientId,
                IpAddress = ipAddress,
                Action = "CONNECTION_REQUEST",
                Success = true,
                Details = $"Connection requested to {request.TargetClientId}"
            };

            _context.ConnectionLogs.Add(log);
            await _context.SaveChangesAsync();

            return new ConnectionResponse
            {
                Success = true,
                Message = "Connection request sent",
                SessionId = Guid.NewGuid().ToString()
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

    public async Task UpdateClientActivityAsync(string clientId)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

        if (client != null)
        {
            client.LastSeen = DateTime.UtcNow;
            client.IsOnline = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetClientOfflineAsync(string clientId)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

        if (client != null)
        {
            client.IsOnline = false;
            client.LastSeen = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
