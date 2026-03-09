using LoginNet.Application.UseCases.Notes;
using LoginNet.Contracts.DTOs;
using LoginNet.Application.Common.Interfaces;
using LoginNet.WebApi.Mappers;

namespace LoginNet.WebApi.Endpoints
{
    public static class NoteEndpoints
    {
        public static RouteGroupBuilder MapNoteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/note");

            group.MapGet("", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetAllNotesQuery());
                return Results.Ok(result.Value?.Select(n => n.ToGetDTO()));
            });

            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetNoteByIdQuery(id));
                if (!result.IsSuccess)
                    return Results.NotFound();
                return Results.Ok(result.Value?.ToGetDTO());
            });

            group.MapPost("", async (NotePostDTO note, IMediator mediator) =>
            {
                var result = await mediator.Send(new CreateNoteCommand(note.Title, note.Content));
                
                if (!result.IsSuccess)
                    return Results.BadRequest(result.ErrorMessage);

                return Results.Created($"/note/{result.Value!.Id}", result.Value.ToGetDTO());
            });

            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new DeleteNoteCommand(id));
                if (!result.IsSuccess)
                    return Results.NotFound(result.ErrorMessage);
                return Results.NoContent();
            });
            
            return group;
        }
    }
}
