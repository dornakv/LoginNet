using LoginNet.Domain.Entities;

namespace LoginNet.Domain.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<bool> AnyByNameAsync(string name);
        Task<bool> ExistsAsync(int roleId);
        new Task<List<Role>> GetAllAsync();
    }
}
