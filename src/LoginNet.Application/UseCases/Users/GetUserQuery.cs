using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Users
{
    public record GetUserQuery(int UserId) : IRequest<Result<User>>;

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetWithRoleAsync(request.UserId);
            if (user == null) return Result<User>.Failure("User not found.");
            return Result<User>.Success(user);
        }
    }
}
