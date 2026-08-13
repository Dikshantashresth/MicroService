using AuthService.DTO;
using AuthService.Repository;
using BCrypt.Net;
using FluentValidation;
using AuthService.Helpers;
using System.Linq;
using AuthService.Model;

namespace AuthService.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IValidator<LoginReq> _loginvalidator;
        private readonly IValidator<User> _reqvalidator;
        private readonly IUnitofWork _unitofwork;
        private readonly ITokenHelper _tokenhelper;
        public AuthServices(ITokenHelper tokenhelper, IUnitofWork unitofWork, IValidator<LoginReq> loginvalidator, IValidator<User> reqvalidator)
        {
            _unitofwork = unitofWork;
            _tokenhelper = tokenhelper;
            _loginvalidator = loginvalidator;
            _reqvalidator = reqvalidator;
        }

        public async Task<ApiResponse> LoginAsync(LoginReq entity)
        {
            var validationResult = await _loginvalidator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.Select(x => x.ErrorMessage);
                var message = string.Join("; ", error);
                return new ApiResponse(400,message);
            }

            var userExists = await _unitofwork.Users.GetByEmail(entity.Email);
            if (userExists == null)
                return new ApiResponse(404, "Invalid Email or Password");

            bool passwordMatches = BCrypt.Net.BCrypt.Verify(entity.Password, userExists.Password);
            if (!passwordMatches)
                return new ApiResponse(401,"Invalid Email or Password");
            var token = _tokenhelper.GenerateJwtToken(userExists);
            var result = new LoginRes
            {
                Name = userExists.Name,
                Email = userExists.Email,
                Token = token
            };

            return new ApiResponse(result, "Sucessfully Registered", 200);
        }

        public async Task<ApiResponse> RegisterAsync(User entity)
        {
            var validationResult = await _reqvalidator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.Select(t => t.ErrorMessage);
                var message = string.Join("|", error);
                return new ApiResponse(400, message);
            }
            var userExists = await _unitofwork.Users.GetByEmail(entity.Email);
            if (userExists !=null) return new ApiResponse(400, "Please Login User Already Exists");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password);
            entity.Password = hashedPassword;
            await _unitofwork.Users.AddAsync(entity);
            await _unitofwork.SaveChanges();
            return new ApiResponse("Registered Successfully", 200);
        }
    }
}
