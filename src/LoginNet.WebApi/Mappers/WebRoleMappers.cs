using LoginNet.Application.Common.Models;
using LoginNet.Contracts.DTOs;
using LoginNet.Domain.Entities;

namespace LoginNet.WebApi.Mappers
{
    public static class WebRoleMappers
    {
        public static RoleGetDTO ToGetDTO(this RoleResponse response)
        {
            return new RoleGetDTO
            {
                Id = response.Id,
                Name = response.Name,
                CanRegisterUsers = response.CanRegisterUsers,
                CanCreateRoles = response.CanCreateRoles
            };
        }

        public static RoleGetDTO ToGetDTO(this Role role)
        {
            return new RoleGetDTO
            {
                Id = role.Id,
                Name = role.Name,
                CanRegisterUsers = role.CanRegisterUsers,
                CanCreateRoles = role.CanCreateRoles
            };
        }
    }
}
