using LoginNet.Domain.Entities;
using LoginNet.Application.Common.Models;

namespace LoginNet.Application.Mappers
{
    public static class NoteMappers
    {
        public static NoteResponse ToNoteResponse(this Note note)
        {
            return new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
            };
        }
    }
}
