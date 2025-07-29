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
}
