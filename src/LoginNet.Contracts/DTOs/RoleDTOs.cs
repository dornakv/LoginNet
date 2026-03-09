namespace LoginNet.Contracts.DTOs
{
    public class RoleGetDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool CanRegisterUsers { get; set; }
        public bool CanCreateRoles { get; set; }
    }

    public class RolePostDTO
    {
        public required string Name { get; set; }
        public bool CanRegisterUsers { get; set; }
        public bool CanCreateRoles { get; set; }
        public int? OwnerId { get; set; }
    }
}
