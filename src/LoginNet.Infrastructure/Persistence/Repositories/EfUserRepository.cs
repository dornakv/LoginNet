using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoginNet.Infrastructure.Persistence.Repositories
{
    public class EfUserRepository : EfRepository<User>, IUserRepository
    {
        public EfUserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<int> CountByRoleIdAsync(int roleId)
        {
            return await _context.Users.CountAsync(u => u.RoleId == roleId);
        }

        public async Task<bool> AnyByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User?> GetWithRoleAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
