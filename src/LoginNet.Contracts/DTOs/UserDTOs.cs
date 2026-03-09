namespace LoginNet.Contracts.DTOs
{
    public class UserGetDTO
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public int RoleId { get; set; }
    }
}
