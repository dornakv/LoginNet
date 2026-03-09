using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoginNet.Infrastructure.Persistence.Repositories
{
    public class EfNoteRepository : EfRepository<Note>, INoteRepository
    {
        public EfNoteRepository(AppDbContext context) : base(context) { }

        public async Task<List<Note>> GetNotesForUserAsync(int userId, HashSet<int> roleIds)
        {
            return await _context.Notes
                .Where(n => n.ReadUserAcl.Contains(userId) || n.ReadRoleAcl.Any(a => roleIds.Contains(a)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Note?> GetNoteForUserReadAsync(int id, int userId, HashSet<int> roleIds)
        {
            return await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && (n.ReadUserAcl.Contains(userId) || n.ReadRoleAcl.Any(a => roleIds.Contains(a))));
        }

        public async Task<Note?> GetNoteForUserWriteAsync(int id, int userId, HashSet<int> roleIds)
        {
            return await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && (n.WriteUserAcl.Contains(userId) || n.WriteRoleAcl.Any(a => roleIds.Contains(a))));
        }
    }
}
