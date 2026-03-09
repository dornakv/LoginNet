using LoginNet.Domain.Entities;
using LoginNet.Domain.Interfaces;

namespace LoginNet.Domain.Services
{
    public class RoleDomainService : IRoleDomainService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleDomainService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> GetAccessibleRolesAsync(int roleId)
        {
            var allRoles = await _roleRepository.GetAllAsync();
            var accessibleRoles = new List<Role>();
            var visited = new HashSet<int>();

            var queue = new Queue<int>();
            queue.Enqueue(roleId);

            while (queue.Count > 0)
            {
                int currentRoleId = queue.Dequeue();

                if (visited.Contains(currentRoleId))
                    continue;

                visited.Add(currentRoleId);

                var role = allRoles.FirstOrDefault(r => r.Id == currentRoleId);
                if (role != null)
                {
                    accessibleRoles.Add(role);

                    var ownedRoles = allRoles.Where(r => r.OwnerId == currentRoleId).Select(r => r.Id);
                    foreach (var ownedRoleId in ownedRoles)
                    {
                        if (!visited.Contains(ownedRoleId))
                            queue.Enqueue(ownedRoleId);
                    }
                }
            }

            return accessibleRoles;
        }

        public async Task<HashSet<int>> GetAccessibleRoleIdsAsync(int roleId)
        {
            var accessibleRoles = await GetAccessibleRolesAsync(roleId);
            return accessibleRoles.Select(r => r.Id).ToHashSet();
        }

        public async Task<bool> CanRegisterUsersInRoleAsync(User user, int targetRoleId)
        {
            if (user?.Role == null)
                return false;

            var accessibleRoles = await GetAccessibleRolesAsync(user.Role.Id);
            var accessibleRoleIds = accessibleRoles.Select(r => r.Id).ToHashSet();

            if (!accessibleRoleIds.Contains(targetRoleId))
                return false;

            if (user.Role.CanRegisterUsers)
                return true;

            foreach (var role in accessibleRoles.Where(r => r.CanRegisterUsers))
            {
                var roleAndDescendants = await GetAccessibleRolesAsync(role.Id);
                if (roleAndDescendants.Any(r => r.Id == targetRoleId))
                    return true;
            }

            return false;
        }

        public async Task<bool> CanCreateRoleWithOwnerAsync(User user, int ownerRoleId)
        {
            if (user?.Role == null)
                return false;

            var accessibleRoles = await GetAccessibleRolesAsync(user.Role.Id);
            var accessibleRoleIds = accessibleRoles.Select(r => r.Id).ToHashSet();

            if (!accessibleRoleIds.Contains(ownerRoleId))
                return false;

            if (user.Role.CanCreateRoles)
                return true;

            foreach (var role in accessibleRoles.Where(r => r.CanCreateRoles))
            {
                var roleAndDescendants = await GetAccessibleRolesAsync(role.Id);
                if (roleAndDescendants.Any(r => r.Id == ownerRoleId))
                    return true;
            }

            return false;
        }
    }
}
