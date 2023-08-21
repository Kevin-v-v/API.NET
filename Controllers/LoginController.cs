
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using BankAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
  

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly IConfiguration _config;
    public LoginController(LoginService loginService, IConfiguration config){
        _loginService = loginService;
        _config = config;
    }

    [HttpPost("authenticateAdmin")]
    public async Task<IActionResult> LoginAdmin(AdminDto adminDto){
        Administrator? admin = await _loginService.GetAdmin(adminDto);
        if(admin == null){
            return BadRequest(new { message = "Credenciales inválidas." });
        }
        string jwtToken = GenerateAdminToken(admin);

        return Ok( new { token = jwtToken} );
    }

    [HttpPost("authenticateClient")]
    public async Task<IActionResult> LoginClient(ClientDto clientDto){
        Client? client = await _loginService.GetClient(clientDto);
        if(client == null){
            return BadRequest(new { message = "Credenciales inválidas." });
        }
        string jwtToken = GenerateClientToken(client);

        return Ok( new { token = jwtToken} );
    }

    private string GenerateAdminToken(Administrator admin){
        var claims = new[] {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType", admin.AdminType)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:AdminKey").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims : claims,
            expires : DateTime.Now.AddMinutes(60),
            signingCredentials : creds
        );
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
    
        return token;
    }   
    private string GenerateClientToken(Client client){
        var claims = new[] {
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email, client.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:ClientKey").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims : claims,
            expires : DateTime.Now.AddMinutes(60),
            signingCredentials : creds
        );
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
    
        return token;
    }   
}