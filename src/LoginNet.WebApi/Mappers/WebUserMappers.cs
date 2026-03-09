using LoginNet.Application.Common.Models;
using LoginNet.Contracts.DTOs;

namespace LoginNet.WebApi.Mappers
{
    public static class WebUserMappers
    {
        public static UserGetDTO ToGetDTO(this UserResponse response)
        {
            return new UserGetDTO
            {
                Id = response.Id,
                Username = response.Username,
                RoleId = response.RoleId
            };
        }
    }
}
