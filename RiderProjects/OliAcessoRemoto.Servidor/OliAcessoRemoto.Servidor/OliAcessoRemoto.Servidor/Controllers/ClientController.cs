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
}
