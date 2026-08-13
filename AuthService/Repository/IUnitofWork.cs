namespace AuthService.Repository
{
    public interface IUnitofWork
    {
        IAuthRepository Users { get; set; }
        Task<int> SaveChanges();

    }
}
