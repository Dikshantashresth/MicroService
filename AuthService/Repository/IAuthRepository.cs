using AuthService.DTO;
using AuthService.Model;

namespace AuthService.Repository
{
    public interface IAuthRepository
    {
        Task AddAsync(User entity);
        Task<User> GetByEmail(string email);
        

    }
}
