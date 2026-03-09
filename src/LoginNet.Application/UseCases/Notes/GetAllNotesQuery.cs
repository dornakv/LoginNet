using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common.Models;
using LoginNet.Application.Mappers;
using LoginNet.Application.Common;
using LoginNet.Application.UseCases.Users;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Notes
{
    public record GetAllNotesQuery() : IRequest<Result<List<NoteResponse>>>;

    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, Result<List<NoteResponse>>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IRoleDomainService _roleDomainService;
        private readonly IMediator _mediator;

        public GetAllNotesQueryHandler(
            INoteRepository noteRepository, 
            IRoleDomainService roleDomainService,
            IMediator mediator)
        {
            _noteRepository = noteRepository;
            _roleDomainService = roleDomainService;
            _mediator = mediator;
        }

        public async Task<Result<List<NoteResponse>>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            var userResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (!userResult.IsSuccess)
                 return Result<List<NoteResponse>>.Failure("User not authenticated.");

            var user = userResult.Value;

            if (user == null)
                return Result<List<NoteResponse>>.Failure("User not found.");

            HashSet<int> userAccessibleRoleIds = await _roleDomainService.GetAccessibleRoleIdsAsync(user.RoleId);
            var notes = await _noteRepository.GetNotesForUserAsync(user.Id, userAccessibleRoleIds);
            return Result<List<NoteResponse>>.Success(notes.Select(n => n.ToNoteResponse()).ToList());
        }
    }
}
