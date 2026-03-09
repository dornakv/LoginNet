using FluentValidation;

namespace LoginNet.Application.UseCases.Roles
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}