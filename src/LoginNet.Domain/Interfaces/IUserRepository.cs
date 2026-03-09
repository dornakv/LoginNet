using LoginNet.Domain.Entities;

namespace LoginNet.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<int> CountByRoleIdAsync(int roleId);
        Task<bool> AnyByUsernameAsync(string username);
        Task<User?> GetWithRoleAsync(int userId);
    }
}
