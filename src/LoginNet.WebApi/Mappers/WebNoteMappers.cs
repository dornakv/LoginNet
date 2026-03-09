using LoginNet.Application.Common.Models;
using LoginNet.Contracts.DTOs;

namespace LoginNet.WebApi.Mappers
{
    public static class WebNoteMappers
    {
        public static NoteGetDTO ToGetDTO(this NoteResponse response)
        {
            return new NoteGetDTO
            {
                Id = response.Id,
                Title = response.Title,
                Content = response.Content,
                CreatedAt = response.CreatedAt
            };
        }
    }
}
