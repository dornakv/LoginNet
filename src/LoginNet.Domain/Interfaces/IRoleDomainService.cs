using LoginNet.Domain.Entities;

namespace LoginNet.Domain.Interfaces
{
    public interface IRoleDomainService
    {
        Task<List<Role>> GetAccessibleRolesAsync(int roleId);
        Task<HashSet<int>> GetAccessibleRoleIdsAsync(int roleId);
        Task<bool> CanRegisterUsersInRoleAsync(User user, int targetRoleId);
        Task<bool> CanCreateRoleWithOwnerAsync(User user, int ownerRoleId);
    }
}
