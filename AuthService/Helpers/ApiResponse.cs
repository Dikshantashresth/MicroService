namespace AuthService.Helpers
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Status { get; set; }
        public object Data { get; set; }
        public ApiResponse(object data, string message = "Success", int statusCode = 200)
        {
            Success = true;
            Message = message;
            Status = statusCode;
            Data = data;
        }
        public ApiResponse(string message, int statusCode = 200)
        {
            Success = true;
            Message = message;
            Status = statusCode;
            Data = null;
        }
        public ApiResponse(int statusCode, string errorMessage)
        {
            Success = false;
            Message = errorMessage;
            Status = statusCode;
            Data = null;
        }
    }
}
