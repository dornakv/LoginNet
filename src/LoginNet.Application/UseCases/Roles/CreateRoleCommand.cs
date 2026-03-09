using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common.Models;
using LoginNet.Application.Common.Enums;
using LoginNet.Application.Common;
using LoginNet.Application.Mappers;
using LoginNet.Application.UseCases.Users;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Roles
{
    public record CreateRoleCommand(string Name, bool CanRegisterUsers, bool CanCreateRoles, int? OwnerId = null) : IRequest<Result<RoleResponse>>;

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleResponse>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleDomainService _roleDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateRoleCommandHandler(
            IRoleRepository roleRepository, 
            IRoleDomainService roleDomainService, 
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _roleRepository = roleRepository;
            _roleDomainService = roleDomainService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Result<RoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (!userResult.IsSuccess)
                 return Result<RoleResponse>.Failure("User not authenticated.", RoleCreationError.UserNotFound);

            var user = userResult.Value;

            if (user?.Role == null)
                return Result<RoleResponse>.Failure("User or role not found.", RoleCreationError.UserNotFound);

            if (!await _roleDomainService.CanCreateRoleWithOwnerAsync(user, request.OwnerId ?? user.RoleId))
                return Result<RoleResponse>.Failure("Insufficient permissions.", RoleCreationError.InsufficientPermissions);

            if (await _roleRepository.AnyByNameAsync(request.Name))
                return Result<RoleResponse>.Failure("Role name already exists.", RoleCreationError.DuplicateName);

            var role = new Role
            {
                Name = request.Name,
                CanRegisterUsers = request.CanRegisterUsers,
                CanCreateRoles = request.CanCreateRoles,
                OwnerId = request.OwnerId
            };

            await _roleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return Result<RoleResponse>.Success(role.ToRoleResponse());
        }
    }
}
