namespace AuthService.DTO
{
    public class LoginReq
    {
        /// <summary>
        /// Unique email used for login
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password used for login
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
