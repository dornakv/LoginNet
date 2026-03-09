using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;
using LoginNet.Application.Common.Models;
using LoginNet.Application.Mappers;
using LoginNet.Application.Common;
using LoginNet.Application.UseCases.Users;
using LoginNet.Application.Common.Interfaces;

namespace LoginNet.Application.UseCases.Roles
{
    public record GetRoleByIdQuery(int Id) : IRequest<Result<RoleResponse>>;

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleResponse>>
    {
        private readonly IRoleDomainService _roleDomainService;
        private readonly IMediator _mediator;

        public GetRoleByIdQueryHandler(
            IRoleDomainService roleDomainService,
            IMediator mediator)
        {
            _roleDomainService = roleDomainService;
            _mediator = mediator;
        }

        public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var userResult = await _mediator.Send(new GetAuthenticatedUserQuery());
            if (!userResult.IsSuccess)
                 return Result<RoleResponse>.Failure("User not authenticated.");

            var user = userResult.Value;

            if (user == null)
                return Result<RoleResponse>.Failure("User not found.");

            List<Role> userAccessibleRoles = await _roleDomainService.GetAccessibleRolesAsync(user.RoleId);
            var role = userAccessibleRoles.FirstOrDefault(r => r.Id == request.Id);
            if (role == null) return Result<RoleResponse>.Failure("Role not found or access denied.");
            return Result<RoleResponse>.Success(role.ToRoleResponse());
        }
    }
}
