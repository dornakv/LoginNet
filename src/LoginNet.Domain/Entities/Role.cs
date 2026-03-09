namespace LoginNet.Domain.Entities
{
    /// <summary>
    /// User from owner role has all the same rights as user from the ownee role
    /// Rights set directly on role (not acls), have only meaning within this role
    /// </summary>
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        /// TODO make tests of following:
        /// User in this role can create users in any sub-role, which has CanCreateUsers true, even when here he has CanCreateUsers false
        /// If any sub-role has CanCreateUsers true - user can create users in ANY sub-role of the sub-role (even those with CanCreateUsers false)
        /// If User has CanCreateUsers true here, he can also create users for this role, and ANY sub-role (even those with CanCreateUsers false)
        public bool CanRegisterUsers { get; set; }

        /// TODO make tests of following:
        /// Similar rules as with CanRegisterUsers should apply
        /// Sub-role could have CanCreateRoles = true even when we have CanCreateRoles = false here.. it could be changed on this role after creation of sub-role, or higher role could force sub-role creation and assign us as its owner.
        /// While we then shouldn't be able to create new sub-roles of this role, we should be able to create sub-roles of a role which has right to create roles...
        public bool CanCreateRoles { get; set; }

        public int? OwnerId { get; set; }
        public Role? Owner { get; set; }

        // Navigation property for users in this role
        public ICollection<User>? Users { get; set; }
    }
}
