using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(AuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }
     

     // login route 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var response = await _authService.LoginAsync(request);
        if (response == null) return Unauthorized();
        return Ok(response);
    }
   

     // register route 
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _authService.RegisterAsync(request);
        if (!result.Success) return BadRequest(new { Errors = result.Errors });
        return Created();
    }

// [AllowAnonymous]
//     [HttpPost("refresh")]
//     public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
//     {
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
        
//         var response = await _authService.RefreshTokenAsync(request);
//         if (response == null) return Unauthorized();
//         return Ok(response);
//     } 
     

     // get users route with authorization 
    [HttpGet("users")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.ToList();
        var userDtos = new List<UserDto>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "No Role"
            });
        }
        
        return Ok(userDtos);
    }
 

 // getting users by id 
  [HttpPut("users/{id}")]
[Authorize(Roles = "Owner")]
public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
{
    var user = await _userManager.FindByIdAsync(id);
    if (user == null) return NotFound("User not found");

  
    if (!string.IsNullOrWhiteSpace(request.Email))
    {
        var emailResult = await _userManager.SetEmailAsync(user, request.Email);
        if (!emailResult.Succeeded) return BadRequest(emailResult.Errors);

        var usernameResult = await _userManager.SetUserNameAsync(user, request.Email);
        if (!usernameResult.Succeeded) return BadRequest(usernameResult.Errors);
    }

 
    if (!string.IsNullOrWhiteSpace(request.Password))
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
        if (!passResult.Succeeded) return BadRequest(passResult.Errors);
    }


    if (!string.IsNullOrWhiteSpace(request.Role))
    {
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
    }

    return Ok("User updated successfully.");
} 

// delete user . 
    [HttpDelete("users/{id}")]
[Authorize(Roles = "Owner")]
public async Task<IActionResult> DeleteUser(string id)
{
    var user = await _userManager.FindByIdAsync(id);
    if (user == null) return NotFound("User not found");

    var result = await _userManager.DeleteAsync(user);
    if (!result.Succeeded) return BadRequest(result.Errors);

    return Ok("User deleted successfully.");
}
}