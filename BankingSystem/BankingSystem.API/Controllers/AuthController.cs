using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public AuthController()
    {
        _httpClient = new HttpClient(); // Manually create HttpClient instance
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] AuthRequest authRequest)
    {
        var tokenEndpoint = "http://localhost:8080/realms/BankingSystem/protocol/openid-connect/token";
        
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "banking-system-api"),
            new KeyValuePair<string, string>("client_secret", "gOWUDWecQqz2Iala3Nk1RJ2wzWLhWMpe"),
            new KeyValuePair<string, string>("username", authRequest.Username),
            new KeyValuePair<string, string>("password", authRequest.Password)
        });

        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        
        if (!response.IsSuccessStatusCode)
        {
            return Unauthorized();
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return Ok(tokenResponse);
    }
}

public class AuthRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class TokenResponse
{
    public string Access_Token { get; set; }
    public int Expires_In { get; set; }
    public string Refresh_Token { get; set; }
    public string Token_Type { get; set; }
    public int Refresh_Expires_In { get; set; }
}
