namespace AuthService.DTO
{
    /// <summary>
    /// Defines the response after login
    /// </summary>
    public class LoginRes
    {
        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// UserName of the user
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
