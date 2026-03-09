using LoginNet.Domain.Interfaces;
using LoginNet.Application.Interfaces;
using LoginNet.Application.Common;
using FluentValidation;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Auth
{
    public record LoginUserCommand(string Username, string Password) : IRequest<Result<string>>;

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public LoginUserCommandHandler(
            IUserRepository userRepository, 
            ITokenService tokenService, 
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !_passwordService.VerifyPassword(user, user.PasswordHash, request.Password))
                return Result<string>.Failure("Invalid username or password.");

            var token = _tokenService.GenerateToken(user);
            return Result<string>.Success(token);
        }
    }
}
