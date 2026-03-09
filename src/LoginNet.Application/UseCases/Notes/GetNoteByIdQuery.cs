using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common.Models;
using LoginNet.Application.Mappers;
using LoginNet.Application.Common;
using LoginNet.Application.UseCases.Users;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Notes
{
    public record GetNoteByIdQuery(int Id) : IRequest<Result<NoteResponse>>;

    public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Result<NoteResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IRoleDomainService _roleDomainService;
        private readonly IMediator _mediator;

        public GetNoteByIdQueryHandler(
            INoteRepository noteRepository, 
            IRoleDomainService roleDomainService,
            IMediator mediator)
        {
            _noteRepository = noteRepository;
            _roleDomainService = roleDomainService;
            _mediator = mediator;
        }

        public async Task<Result<NoteResponse>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var userResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (!userResult.IsSuccess)
                 return Result<NoteResponse>.Failure("User not authenticated.");

            var user = userResult.Value;

            if (user == null)
                return Result<NoteResponse>.Failure("User not found.");

            HashSet<int> userAccessibleRoleIds = await _roleDomainService.GetAccessibleRoleIdsAsync(user.RoleId);
            var note = await _noteRepository.GetNoteForUserReadAsync(request.Id, user.Id, userAccessibleRoleIds);
            if (note == null) return Result<NoteResponse>.Failure("Note not found or access denied.");
            return Result<NoteResponse>.Success(note.ToNoteResponse());
        }
    }
}
