using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common;
using LoginNet.Application.Common.Interfaces;
using LoginNet.Application.Interfaces;

namespace LoginNet.Application.UseCases.Users
{
    public record GetAuthenticatedUserQuery : IRequest<Result<User>>;

    public class GetAuthenticatedUserQueryHandler : IRequestHandler<GetAuthenticatedUserQuery, Result<User>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAuthenticatedUserQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<User>> Handle(GetAuthenticatedUserQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
                return Result<User>.Failure("User not authenticated.");

            var user = await _userRepository.GetWithRoleAsync(_currentUserService.UserId.Value);
            if (user == null) return Result<User>.Failure("User not found.");
            
            return Result<User>.Success(user);
        }
    }
}
