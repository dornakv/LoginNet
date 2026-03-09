using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common;
using LoginNet.Application.Common.Enums;
using LoginNet.Application.Common.Models;
using LoginNet.Application.Mappers;
using LoginNet.Application.Interfaces;
using FluentValidation;
using LoginNet.Application.Common.Interfaces;
using LoginNet.Application.UseCases.Users;

namespace LoginNet.Application.UseCases.Auth
{
    public record RegisterUserCommand(string Username, string Password, int? RoleId = null) : IRequest<Result<UserResponse>>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleDomainService _roleDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public RegisterUserCommandHandler(
            IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            IRoleDomainService roleDomainService, 
            IUnitOfWork unitOfWork,
            IPasswordService passwordService,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleDomainService = roleDomainService;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Result<UserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var username = request.Username;
            var password = request.Password;
            var roleId = request.RoleId;

            User? currentUser = null;
            var authUserResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (authUserResult.IsSuccess)
            {
                currentUser = authUserResult.Value;
            }

            if (currentUser == null)
            {
                var adminUserCount = await _userRepository.CountByRoleIdAsync(1);
                if (adminUserCount > 0)
                    return Result<UserResponse>.Failure("Insufficient permissions.", RegistrationError.InsufficientPermissions);
                roleId = 1;
            }
            else
            {
                if (roleId == null)
                    return Result<UserResponse>.Failure("Role is required.", RegistrationError.RoleRequired);

                bool canRegister = await _roleDomainService.CanRegisterUsersInRoleAsync(currentUser, roleId.Value);
                if (!canRegister)
                    return Result<UserResponse>.Failure("Insufficient permissions.", RegistrationError.InsufficientPermissions);
            }

            if (await _userRepository.AnyByUsernameAsync(username))
                return Result<UserResponse>.Failure("Username already exists.", RegistrationError.UsernameAlreadyExists);

            if (roleId == null)
                return Result<UserResponse>.Failure("Role is required.", RegistrationError.RoleRequired);

            var roleExists = await _roleRepository.ExistsAsync(roleId.Value);
            if (!roleExists)
                return Result<UserResponse>.Failure("Specified role does not exist.", RegistrationError.InvalidRoleId);

            var user = User.Create(username, roleId.Value, string.Empty);
            user.UpdatePasswordHash(_passwordService.HashPassword(user, password));
            
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return Result<UserResponse>.Success(user.ToUserResponse());
        }
    }
}
