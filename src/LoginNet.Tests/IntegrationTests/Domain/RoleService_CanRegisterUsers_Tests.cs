using LoginNet.Domain.Services;
using LoginNet.Tests.Helpers;
using Xunit;

namespace LoginNet.Tests.IntegrationTests.Domain
{
    /// <summary>
    /// Tests for CanRegisterUsers permission inheritance logic as described in TODO comments
    /// </summary>
    public class RoleService_CanRegisterUsers_Tests : DatabaseTestBase
    {
        private readonly RoleDomainService _roleService;

        public RoleService_CanRegisterUsers_Tests()
        {
            _roleService = new RoleDomainService(_roleRepository);
        }

        /// <summary>
        /// Test: If User has CanRegisterUsers=true, they can register users in their own role
        /// </summary>
        [Fact]
        public async Task UserWithPermission_CanRegisterInOwnRole()
        {
            // Arrange
            var role = await CreateRoleAsync("AdminRole", canRegisterUsers: true);
            var user = await CreateUserAsync("admin", role.Id);

            // Act
            var canRegister = await _roleService.CanRegisterUsersInRoleAsync(user, role.Id);

            // Assert
            Assert.True(canRegister, "User with CanRegisterUsers=true should be able to register users in their own role");
        }

        /// <summary>
        /// Test: If User has CanRegisterUsers=true, they can register users in ANY accessible sub-role (even if sub-role has CanRegisterUsers=false)
        /// </summary>
        [Fact]
        public async Task UserWithPermission_CanRegisterInAnySubRole_EvenWhenSubRoleHasNoPermission()
        {
            // Arrange
            var parentRole = await CreateRoleAsync("ParentRole", canRegisterUsers: true);
            var subRole = await CreateRoleAsync("SubRole", canRegisterUsers: false, ownerId: parentRole.Id);
            var user = await CreateUserAsync("admin", parentRole.Id);

            // Act
            var canRegister = await _roleService.CanRegisterUsersInRoleAsync(user, subRole.Id);

            // Assert
            Assert.True(canRegister, "User with CanRegisterUsers=true should be able to register in sub-role even if sub-role has CanRegisterUsers=false");
        }

        /// <summary>
        /// Test: User without CanRegisterUsers cannot register users in their own role
        /// </summary>
        [Fact]
        public async Task UserWithoutPermission_CannotRegisterInOwnRole()
        {
            // Arrange
            var role = await CreateRoleAsync("UserRole", canRegisterUsers: false);
            var user = await CreateUserAsync("user", role.Id);

            // Act
            var canRegister = await _roleService.CanRegisterUsersInRoleAsync(user, role.Id);

            // Assert
            Assert.False(canRegister, "User with CanRegisterUsers=false should not be able to register users in their own role");
        }

        /// <summary>
        /// Test: User in role with CanRegisterUsers=false can register users in a sub-role if that sub-role has CanRegisterUsers=true
        /// This is the key inheritance behavior from TODO comments
        /// </summary>
        [Fact]
        public async Task UserWithoutDirectPermission_CanRegisterInSubRole_WhenSubRoleHasPermission()
        {
            // Arrange
            var parentRole = await CreateRoleAsync("ParentRole", canRegisterUsers: false);
            var subRole = await CreateRoleAsync("SubRole", canRegisterUsers: true, ownerId: parentRole.Id);
            var user = await CreateUserAsync("user", parentRole.Id);

            // Act
            var canRegister = await _roleService.CanRegisterUsersInRoleAsync(user, subRole.Id);

            // Assert
            Assert.True(canRegister, "User should be able to register users in a role that has CanRegisterUsers=true, even if their role has it false");
        }

        /// <summary>
        /// Test: If intermediate sub-role has CanRegisterUsers=true, user can register users in ANY of its descendants (even if they have CanRegisterUsers=false)
        /// This is another key behavior from TODO comments
        /// </summary>
        [Fact]
        public async Task UserInRoleWithoutPermission_CanRegisterInDescendant_WhenIntermediateSubRoleHasPermission()
        {
            // Arrange
            var topRole = await CreateRoleAsync("TopRole", canRegisterUsers: false);
            var midRole = await CreateRoleAsync("MidRole", canRegisterUsers: true, ownerId: topRole.Id);
            var bottomRole = await CreateRoleAsync("BottomRole", canRegisterUsers: false, ownerId: midRole.Id);
            var user = await CreateUserAsync("user", topRole.Id);

            // Act
            var canRegisterUnderMid = await _roleService.CanRegisterUsersInRoleAsync(user, midRole.Id);
            var canRegisterUnderBottom = await _roleService.CanRegisterUsersInRoleAsync(user, bottomRole.Id);

            // Assert
            Assert.True(canRegisterUnderMid, "User should be able to register in mid-role that has CanRegisterUsers=true");
            Assert.True(canRegisterUnderBottom, "User should be able to register in bottom-role because mid-role (its parent) has CanRegisterUsers=true");
        }

        /// <summary>
        /// Test: User cannot register users in roles they don't have access to (not in their hierarchy)
        /// </summary>
        [Fact]
        public async Task User_CannotRegisterInUnrelatedRole()
        {
            // Arrange
            var role1 = await CreateRoleAsync("Role1", canRegisterUsers: true);
            var role2 = await CreateRoleAsync("Role2", canRegisterUsers: false); // Unrelated role
            var user = await CreateUserAsync("user", role1.Id);

            // Act
            var canRegister = await _roleService.CanRegisterUsersInRoleAsync(user, role2.Id);

            // Assert
            Assert.False(canRegister, "User should not be able to register users in roles outside their hierarchy");
        }
    }
}
