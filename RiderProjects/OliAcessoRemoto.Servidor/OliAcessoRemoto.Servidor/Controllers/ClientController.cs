using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OliAcessoRemoto.Servidor.Models.DTOs;
using OliAcessoRemoto.Servidor.Services;

namespace OliAcessoRemoto.Servidor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientController> _logger;

    public ClientController(IClientService clientService, ILogger<ClientController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ClientRegistrationResponse>> RegisterClient(ClientRegistrationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _clientService.RegisterClientAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterClient endpoint");
            return StatusCode(500, new ClientRegistrationResponse 
            { 
                Success = false, 
                Message = "Internal server error" 
            });
        }
    }

    [HttpGet("status/{clientId}")]
    [Authorize]
    public async Task<ActionResult<ClientStatusResponse>> GetClientStatus(string clientId)
    {
        try
        {
            var response = await _clientService.GetClientStatusAsync(clientId);
            
            if (response == null)
            {
                return NotFound(new { Message = "Client not found" });
            }
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetClientStatus endpoint for client {ClientId}", clientId);
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPost("connection/request")]
    [Authorize]
    public async Task<ActionResult<ConnectionResponse>> RequestConnection(ConnectionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _clientService.RequestConnectionAsync(request);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RequestConnection endpoint");
            return StatusCode(500, new ConnectionResponse 
            { 
                Success = false, 
                Message = "Internal server error" 
            });
        }
    }

    [HttpGet("clients/online")]
    [Authorize]
    public async Task<ActionResult<List<OnlineClientDto>>> GetOnlineClients()
    {
        try
        {
            var clients = await _clientService.GetOnlineClientsAsync();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOnlineClients endpoint");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPost("validate")]
    [Authorize]
    public async Task<ActionResult<bool>> ValidateClient([FromBody] string clientId)
    {
        try
        {
            var isValid = await _clientService.ValidateClientAsync(clientId);
            return Ok(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ValidateClient endpoint");
            return StatusCode(500, false);
        }
    }
}
