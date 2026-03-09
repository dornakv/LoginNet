using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common;
using LoginNet.Application.UseCases.Users;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Notes
{
    public record DeleteNoteCommand(int Id) : IRequest<Result>;

    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Result>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IRoleDomainService _roleDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteNoteCommandHandler(
            INoteRepository noteRepository, 
            IRoleDomainService roleDomainService, 
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _noteRepository = noteRepository;
            _roleDomainService = roleDomainService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (!userResult.IsSuccess)
                 return Result.Failure("User not authenticated.");

            var user = userResult.Value;

            if (user == null)
                return Result.Failure("User not found.");

            HashSet<int> userAccessibleRoleIds = await _roleDomainService.GetAccessibleRoleIdsAsync(user.RoleId);
            var note = await _noteRepository.GetNoteForUserWriteAsync(request.Id, user.Id, userAccessibleRoleIds);
            if (note == null) return Result.Failure("Note not found or access denied.");

            _noteRepository.Delete(note);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
    }
}
