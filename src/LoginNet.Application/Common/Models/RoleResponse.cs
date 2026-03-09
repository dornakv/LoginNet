namespace LoginNet.Application.Common.Models
{
    public class RoleResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool CanRegisterUsers { get; set; }
        public bool CanCreateRoles { get; set; }
    }
}
