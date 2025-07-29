using Microsoft.AspNetCore.Mvc;
using OnliDesk.Server.Models.DTOs;
using OnliDesk.Server.Services;

namespace OnliDesk.Server.Controllers;

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
    public async Task<ActionResult<ClientRegistrationResponse>> RegisterClient([FromBody] ClientRegistrationRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _clientService.RegisterClientAsync(request, ipAddress);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterClient");
            return StatusCode(500, new ClientRegistrationResponse 
            { 
                Success = false, 
                Message = "Internal server error" 
            });
        }
    }

    [HttpGet("status/{clientId}")]
    public async Task<ActionResult<ClientStatusResponse>> GetClientStatus(string clientId)
    {
        try
        {
            var result = await _clientService.GetClientStatusAsync(clientId);
            
            if (result == null)
            {
                return NotFound(new { message = "Client not found" });
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetClientStatus for {ClientId}", clientId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("clients/online")]
    public async Task<ActionResult<List<ClientStatusResponse>>> GetOnlineClients()
    {
        try
        {
            var result = await _clientService.GetOnlineClientsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOnlineClients");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("connection/request")]
    public async Task<ActionResult<ConnectionResponse>> RequestConnection([FromBody] ConnectionRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _clientService.RequestConnectionAsync(request, ipAddress);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RequestConnection");
            return StatusCode(500, new ConnectionResponse 
            { 
                Success = false, 
                Message = "Internal server error" 
            });
        }
    }

    [HttpPost("heartbeat/{clientId}")]
    public async Task<ActionResult> Heartbeat(string clientId)
    {
        try
        {
            await _clientService.UpdateClientActivityAsync(clientId);
            return Ok(new { message = "Heartbeat received" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Heartbeat for {ClientId}", clientId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
