using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common.Models;
using LoginNet.Application.Mappers;
using LoginNet.Application.Common;
using LoginNet.Application.UseCases.Users;
using FluentValidation;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Notes
{
    public record CreateNoteCommand(string Title, string Content) : IRequest<Result<NoteResponse>>;

    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<NoteResponse>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateNoteCommandHandler(
            INoteRepository noteRepository, 
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _noteRepository = noteRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Result<NoteResponse>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (!userResult.IsSuccess)
                 return Result<NoteResponse>.Failure("User not authenticated.");

            var user = userResult.Value;

            if (user == null)
                return Result<NoteResponse>.Failure("User not found.");

            Note note = new()
            {
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                ReadUserAcl = [user.Id],
                WriteUserAcl = [user.Id]
            };
            await _noteRepository.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();
            return Result<NoteResponse>.Success(note.ToNoteResponse());
        }
    }
}
