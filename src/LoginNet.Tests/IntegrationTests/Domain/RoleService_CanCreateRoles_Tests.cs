using LoginNet.Domain.Services;
using LoginNet.Tests.Helpers;
using Xunit;

namespace LoginNet.Tests.IntegrationTests.Domain
{
    /// <summary>
    /// Tests for CanCreateRoles permission inheritance logic as described in TODO comments
    /// Similar rules as CanRegisterUsers
    /// </summary>
    public class RoleService_CanCreateRoles_Tests : DatabaseTestBase
    {
        private readonly RoleDomainService _roleService;

        public RoleService_CanCreateRoles_Tests()
        {
            _roleService = new RoleDomainService(_roleRepository);
        }

        /// <summary>
        /// Test: If User has CanCreateRoles=true, they can create sub-roles of their own role
        /// </summary>
        [Fact]
        public async Task UserWithPermission_CanCreateSubRoleOfOwnRole()
        {
            // Arrange
            var role = await CreateRoleAsync("AdminRole", canCreateRoles: true);
            var user = await CreateUserAsync("admin", role.Id);

            // Act
            var canCreate = await _roleService.CanCreateRoleWithOwnerAsync(user, role.Id);

            // Assert
            Assert.True(canCreate, "User with CanCreateRoles=true should be able to create sub-roles of their own role");
        }

        /// <summary>
        /// Test: If User has CanCreateRoles=true, they can create sub-roles of ANY accessible sub-role (even if sub-role has CanCreateRoles=false)
        /// </summary>
        [Fact]
        public async Task UserWithPermission_CanCreateSubRoleOfAnySubRole_EvenWhenSubRoleHasNoPermission()
        {
            // Arrange
            var parentRole = await CreateRoleAsync("ParentRole", canCreateRoles: true);
            var subRole = await CreateRoleAsync("SubRole", canCreateRoles: false, ownerId: parentRole.Id);
            var user = await CreateUserAsync("admin", parentRole.Id);

            // Act
            var canCreate = await _roleService.CanCreateRoleWithOwnerAsync(user, subRole.Id);

            // Assert
            Assert.True(canCreate, "User with CanCreateRoles=true should be able to create sub-role of sub-role even if sub-role has CanCreateRoles=false");
        }

        /// <summary>
        /// Test: If User has CanCreateRoles=true, they can create sub-roles of deeply nested roles
        /// </summary>
        [Fact]
        public async Task UserWithPermission_CanCreateSubRoleOfDeeplyNestedRoles()
        {
            // Arrange
            var topRole = await CreateRoleAsync("TopRole", canCreateRoles: true);
            var midRole = await CreateRoleAsync("MidRole", canCreateRoles: false, ownerId: topRole.Id);
            var bottomRole = await CreateRoleAsync("BottomRole", canCreateRoles: false, ownerId: midRole.Id);
            var user = await CreateUserAsync("admin", topRole.Id);

            // Act
            var canCreateUnderMid = await _roleService.CanCreateRoleWithOwnerAsync(user, midRole.Id);
            var canCreateUnderBottom = await _roleService.CanCreateRoleWithOwnerAsync(user, bottomRole.Id);

            // Assert
            Assert.True(canCreateUnderMid, "User with CanCreateRoles=true should be able to create sub-role of mid-level role");
            Assert.True(canCreateUnderBottom, "User with CanCreateRoles=true should be able to create sub-role of deeply nested role");
        }

        /// <summary>
        /// Test: User without CanCreateRoles cannot create sub-roles of their own role
        /// </summary>
        [Fact]
        public async Task UserWithoutPermission_CannotCreateSubRoleOfOwnRole()
        {
            // Arrange
            var role = await CreateRoleAsync("UserRole", canCreateRoles: false);
            var user = await CreateUserAsync("user", role.Id);

            // Act
            var canCreate = await _roleService.CanCreateRoleWithOwnerAsync(user, role.Id);

            // Assert
            Assert.False(canCreate, "User with CanCreateRoles=false should not be able to create sub-roles of their own role");
        }

        /// <summary>
        /// Test: User in role with CanCreateRoles=false can create sub-roles of a sub-role if that sub-role has CanCreateRoles=true
        /// This is the key inheritance behavior from TODO comments
        /// </summary>
        [Fact]
        public async Task UserWithoutDirectPermission_CanCreateSubRoleOfSubRole_WhenSubRoleHasPermission()
        {
            // Arrange
            var parentRole = await CreateRoleAsync("ParentRole", canCreateRoles: false);
            var subRole = await CreateRoleAsync("SubRole", canCreateRoles: true, ownerId: parentRole.Id);
            var user = await CreateUserAsync("user", parentRole.Id);

            // Act
            var canCreate = await _roleService.CanCreateRoleWithOwnerAsync(user, subRole.Id);

            // Assert
            Assert.True(canCreate, "User should be able to create sub-role of a role that has CanCreateRoles=true, even if their role has it false");
        }

        /// <summary>
        /// Test: If intermediate sub-role has CanCreateRoles=true, user can create sub-roles of ANY of its descendants (even if they have CanCreateRoles=false)
        /// This is another key behavior from TODO comments
        /// </summary>
        [Fact]
        public async Task UserInRoleWithoutPermission_CanCreateSubRoleOfDescendant_WhenIntermediateSubRoleHasPermission()
        {
            // Arrange
            var topRole = await CreateRoleAsync("TopRole", canCreateRoles: false);
            var midRole = await CreateRoleAsync("MidRole", canCreateRoles: true, ownerId: topRole.Id);
            var bottomRole = await CreateRoleAsync("BottomRole", canCreateRoles: false, ownerId: midRole.Id);
            var user = await CreateUserAsync("user", topRole.Id);

            // Act
            var canCreateUnderMid = await _roleService.CanCreateRoleWithOwnerAsync(user, midRole.Id);
            var canCreateUnderBottom = await _roleService.CanCreateRoleWithOwnerAsync(user, bottomRole.Id);

            // Assert
            Assert.True(canCreateUnderMid, "User should be able to create sub-role of mid-role that has CanCreateRoles=true");
            Assert.True(canCreateUnderBottom, "User should be able to create sub-role of bottom-role because mid-role (its parent) has CanCreateRoles=true");
        }

        /// <summary>
        /// Test: User cannot create sub-roles of roles they don't have access to (not in their hierarchy)
        /// </summary>
        [Fact]
        public async Task User_CannotCreateSubRoleOfUnrelatedRole()
        {
            // Arrange
            var role1 = await CreateRoleAsync("Role1", canCreateRoles: true);
            var role2 = await CreateRoleAsync("Role2", canCreateRoles: false); // Unrelated role
            var user = await CreateUserAsync("user", role1.Id);

            // Act
            var canCreate = await _roleService.CanCreateRoleWithOwnerAsync(user, role2.Id);

            // Assert
            Assert.False(canCreate, "User should not be able to create sub-roles of roles outside their hierarchy");
        }

        /// <summary>
        /// Test: Complex hierarchy with multiple branches - user should respect permission rules in their branch
        /// </summary>
        [Fact]
        public async Task ComplexHierarchy_PermissionRulesApplyCorrectly()
        {
            // Arrange
            var topRole = await CreateRoleAsync("TopRole", canCreateRoles: false);
            var branch1 = await CreateRoleAsync("Branch1", canCreateRoles: true, ownerId: topRole.Id);
            var branch2 = await CreateRoleAsync("Branch2", canCreateRoles: false, ownerId: topRole.Id);
            var leaf1a = await CreateRoleAsync("Leaf1a", canCreateRoles: false, ownerId: branch1.Id);
            var leaf2a = await CreateRoleAsync("Leaf2a", canCreateRoles: false, ownerId: branch2.Id);
            var user = await CreateUserAsync("user", topRole.Id);

            // Act & Assert
            Assert.False(await _roleService.CanCreateRoleWithOwnerAsync(user, topRole.Id),
                "Cannot create sub-role of top role (has no permission)");
            
            Assert.True(await _roleService.CanCreateRoleWithOwnerAsync(user, branch1.Id),
                "Can create sub-role of branch1 (has permission)");
            
            Assert.False(await _roleService.CanCreateRoleWithOwnerAsync(user, branch2.Id),
                "Cannot create sub-role of branch2 (no permission)");
            
            Assert.True(await _roleService.CanCreateRoleWithOwnerAsync(user, leaf1a.Id),
                "Can create sub-role of leaf1a (parent branch1 has permission)");
            
            Assert.False(await _roleService.CanCreateRoleWithOwnerAsync(user, leaf2a.Id),
                "Cannot create sub-role of leaf2a (parent branch2 has no permission)");
        }

        /// <summary>
        /// Test: User at mid-level can create sub-roles based on accessible permissions
        /// </summary>
        [Fact]
        public async Task MidLevelUser_CanCreateSubRolesBasedOnAccessiblePermissions()
        {
            // Arrange
            var topRole = await CreateRoleAsync("TopRole", canCreateRoles: false);
            var midRole = await CreateRoleAsync("MidRole", canCreateRoles: false, ownerId: topRole.Id);
            var bottomWithPerm = await CreateRoleAsync("BottomWithPerm", canCreateRoles: true, ownerId: midRole.Id);
            var bottomLeaf = await CreateRoleAsync("BottomLeaf", canCreateRoles: false, ownerId: bottomWithPerm.Id);
            
            var user = await CreateUserAsync("midUser", midRole.Id);

            // Act & Assert
            Assert.False(await _roleService.CanCreateRoleWithOwnerAsync(user, midRole.Id),
                "Cannot create sub-role of own role (no permission)");
            
            Assert.True(await _roleService.CanCreateRoleWithOwnerAsync(user, bottomWithPerm.Id),
                "Can create sub-role of accessible role that has permission");
            
            Assert.True(await _roleService.CanCreateRoleWithOwnerAsync(user, bottomLeaf.Id),
                "Can create sub-role of leaf because its parent has permission");
        }

        /// <summary>
        /// Test: Sub-role could have CanCreateRoles=true even when parent has it false
        /// User should still be able to create sub-roles of that sub-role
        /// This tests the scenario mentioned in TODO about role permissions changing after creation
        /// </summary>
        [Fact]
        public async Task SubRoleWithPermission_CanBeUsedEvenWhenParentLacksPermission()
        {
            // Arrange
            var parentRole = await CreateRoleAsync("ParentRole", canCreateRoles: false);
            var subRoleWithPerm = await CreateRoleAsync("SubRoleWithPerm", canCreateRoles: true, ownerId: parentRole.Id);
            var subSubRole = await CreateRoleAsync("SubSubRole", canCreateRoles: false, ownerId: subRoleWithPerm.Id);
            
            var user = await CreateUserAsync("user", parentRole.Id);

            // Act
            var canCreateUnderParent = await _roleService.CanCreateRoleWithOwnerAsync(user, parentRole.Id);
            var canCreateUnderSubWithPerm = await _roleService.CanCreateRoleWithOwnerAsync(user, subRoleWithPerm.Id);
            var canCreateUnderSubSub = await _roleService.CanCreateRoleWithOwnerAsync(user, subSubRole.Id);

            // Assert
            Assert.False(canCreateUnderParent, 
                "Cannot create sub-role directly under parent (no permission)");
            Assert.True(canCreateUnderSubWithPerm, 
                "Can create sub-role under sub-role that has permission, even though parent lacks it");
            Assert.True(canCreateUnderSubSub, 
                "Can create sub-role under descendants of role with permission");
        }
    }
}
