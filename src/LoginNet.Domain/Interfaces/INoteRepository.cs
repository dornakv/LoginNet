using LoginNet.Domain.Entities;

namespace LoginNet.Domain.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<List<Note>> GetNotesForUserAsync(int userId, HashSet<int> roleIds);
        Task<Note?> GetNoteForUserReadAsync(int id, int userId, HashSet<int> roleIds);
        Task<Note?> GetNoteForUserWriteAsync(int id, int userId, HashSet<int> roleIds);
    }
}
