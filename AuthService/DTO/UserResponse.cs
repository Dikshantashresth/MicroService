namespace AuthService.DTO
{
    /// <summary>
    /// Reponse structure when user requests their credentials.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Date and time of account created
        /// </summary>
        public DateTime Createdat { get; set; }
    }
}
