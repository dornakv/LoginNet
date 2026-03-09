using LoginNet.Domain.Entities;
using LoginNet.Application.Common.Models;

namespace LoginNet.Application.Mappers
{
    public static class UserMappers
    {
        public static UserResponse ToUserResponse(this User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                RoleId = user.RoleId
            };
        }
    }
}
