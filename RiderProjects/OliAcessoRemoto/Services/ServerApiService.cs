using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using OliAcessoRemoto.Models;

namespace OliAcessoRemoto.Services;

public interface IServerApiService
{
    Task<ClientRegistrationResponse> RegisterClientAsync(string clientName);
    Task<ConnectionResponse> RequestConnectionAsync(string targetClientId, string requesterClientId, string? password = null);
    Task<ClientStatusResponse?> GetClientStatusAsync(string clientId);
    Task<List<OnlineClientDto>> GetOnlineClientsAsync();
    Task<bool> CheckServerHealthAsync();
    void SetAuthToken(string token);
}

public class ServerApiService : IServerApiService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _authToken;

    public ServerApiService(string baseUrl = "http://172.25.63.212:5165")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "OliAcessoRemoto-Client/1.0");
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ClientRegistrationResponse> RegisterClientAsync(string clientName)
    {
        try
        {
            var request = new ClientRegistrationRequest
            {
                Name = clientName,
                ConnectionInfo = JsonSerializer.Serialize(new
                {
                    ip = GetLocalIpAddress(),
                    version = "1.0.0",
                    os = Environment.OSVersion.ToString(),
                    computerName = Environment.MachineName,
                    userName = Environment.UserName
                })
            };

            var response = await _httpClient.PostAsJsonAsync("/api/client/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ClientRegistrationResponse>();
                return result ?? new ClientRegistrationResponse { Success = false, Message = "Invalid response" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ClientRegistrationResponse 
                { 
                    Success = false, 
                    Message = $"Server error: {response.StatusCode} - {errorContent}" 
                };
            }
        }
        catch (Exception ex)
        {
            return new ClientRegistrationResponse 
            { 
                Success = false, 
                Message = $"Connection error: {ex.Message}" 
            };
        }
    }

    public async Task<ConnectionResponse> RequestConnectionAsync(string targetClientId, string requesterClientId, string? password = null)
    {
        try
        {
            var request = new ConnectionRequest
            {
                TargetClientId = targetClientId,
                RequesterClientId = requesterClientId,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/client/connection/request", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ConnectionResponse>();
                return result ?? new ConnectionResponse { Success = false, Message = "Invalid response" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ConnectionResponse 
                { 
                    Success = false, 
                    Message = $"Server error: {response.StatusCode} - {errorContent}" 
                };
            }
        }
        catch (Exception ex)
        {
            return new ConnectionResponse 
            { 
                Success = false, 
                Message = $"Connection error: {ex.Message}" 
            };
        }
    }

    public async Task<ClientStatusResponse?> GetClientStatusAsync(string clientId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/client/status/{clientId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ClientStatusResponse>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<OnlineClientDto>> GetOnlineClientsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/client/clients/online");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<OnlineClientDto>>();
                return result ?? new List<OnlineClientDto>();
            }
            
            return new List<OnlineClientDto>();
        }
        catch
        {
            return new List<OnlineClientDto>();
        }
    }

    public async Task<bool> CheckServerHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private string GetLocalIpAddress()
    {
        try
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            var localIp = host.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return localIp?.ToString() ?? "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
