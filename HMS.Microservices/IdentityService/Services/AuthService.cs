using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace IdentityService.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, HttpClient httpClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _httpClient = httpClient;
    }
     

     // Login request 
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return null;

        var token = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

     

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(15)
        };
    }
    
    // Register request 
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return new RegisterResult { Success = false, Errors = result.Errors.Select(e => e.Description) };

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded) return new RegisterResult { Success = false, Errors = roleResult.Errors.Select(e => e.Description) };

       
        try
        {
            await SendWelcomeEmailAsync(user.Email, user.Email);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send welcome email to {Email}", user.Email);
    
        }

        return new RegisterResult { Success = true };
    }

   
 
    private async Task SendWelcomeEmailAsync(string userEmail, string userName)
    {
        try
        {
            var notificationServiceUrl = _configuration["ServiceUrls:NotificationService"] ?? "http://localhost:5022";
            var requestBody = new { UserEmail = userEmail, UserName = userName };
            
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), 
                System.Text.Encoding.UTF8, "application/json");
            
            Log.Information("Sending welcome email request to {NotificationUrl} for {UserEmail}", 
                notificationServiceUrl, userEmail);
            
            var response = await _httpClient.PostAsync($"{notificationServiceUrl}/api/notifications/welcome", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Warning("Failed to send welcome email through NotificationService. Status: {StatusCode}, Response: {ResponseContent}", 
                    response.StatusCode, errorContent);
            }
            else
            {
                Log.Information("Welcome email sent successfully to {UserEmail}", userEmail);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error sending welcome email to {UserEmail}", userEmail);
            throw;
        }
    }
    

    // create jwt token 
    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Receptionist")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "defaultkey"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}