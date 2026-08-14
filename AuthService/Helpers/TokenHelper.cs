using AuthService.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace AuthService.Helpers;

public interface ITokenHelper
{
    string GenerateJwtToken(User user);
}

public class TokenHelper : ITokenHelper
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenHelper> _logger;

    public TokenHelper(IConfiguration configuration, ILogger<TokenHelper> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    /// <summary>
    /// Generates JWT token from the configuration;
    /// </summary>
    /// <param name="user">Accepts user credientials</param>
    /// <returns>Returns JWT Token</returns>
    public string GenerateJwtToken(User user)
    {

        var jwtSettings = _configuration.GetSection("JWTSettings");
        if (_logger.IsEnabled(LogLevel.Trace))
        {
           
            _logger.LogInformation("Loading JWT configuration section: {JwtSettingsRaw}", jwtSettings.Value);
        }
        _logger.LogInformation($"{jwtSettings.Value}");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryInMinutes = double.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");
        _logger.LogTrace($"{secretKey}, {issuer}, {audience}, {expiryInMinutes}");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };


        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
