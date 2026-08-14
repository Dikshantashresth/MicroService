using AuthService.DTO;
using AuthService.Model;
using AuthService.Services;
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

        /// <summary>
        /// Gets user details from body. Validates it. And registers the user data in database. Returns the data.
        /// </summary>
        /// <param name="request">User Model Data</param>
        /// <returns> Returns registered user details. </returns>
        /// <response code="409">User already Exists</response>
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            var response = await _authService.RegisterAsync(request);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">Login Req data</param>
        /// <returns></returns>
        /// <response code="401">Invalid Credentials</response>
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            var response = await _authService.LoginAsync(request);
            return StatusCode(response.Status, response);
        }
    }
}
