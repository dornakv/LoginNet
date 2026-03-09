using LoginNet.Domain.Entities;
using LoginNet.Application.Common.Models;

namespace LoginNet.Application.Mappers
{
    public static class RoleMappers
    {
        public static RoleResponse ToRoleResponse(this Role role)
        {
            return new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                CanRegisterUsers = role.CanRegisterUsers,
                CanCreateRoles = role.CanCreateRoles,
            };
        }
    }
}
