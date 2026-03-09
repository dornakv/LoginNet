using FluentValidation;

namespace LoginNet.Application.UseCases.Notes
{
    public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
    {
        public CreateNoteCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
