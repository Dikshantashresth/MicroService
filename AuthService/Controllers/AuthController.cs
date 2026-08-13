using AuthService.DTO;
using AuthService.Model;
using AuthService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            var response =  _authService.RegisterAsync(request);
            return Ok(response);
        }
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
    }
}
