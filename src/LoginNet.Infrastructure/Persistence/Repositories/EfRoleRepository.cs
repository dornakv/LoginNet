using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoginNet.Infrastructure.Persistence.Repositories
{
    public class EfRoleRepository : EfRepository<Role>, IRoleRepository
    {
        public EfRoleRepository(AppDbContext context) : base(context) { }

        public async Task<bool> AnyByNameAsync(string name)
        {
            return await _context.Roles.AnyAsync(r => r.Name == name);
        }

        public async Task<bool> ExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.Id == roleId);
        }

        public new async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
