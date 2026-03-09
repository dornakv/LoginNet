namespace LoginNet.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<int> ReadUserAcl { get; set; } = new();
        public List<int> WriteUserAcl { get; set; } = new();

        public List<int> ReadRoleAcl { get; set; } = new();
        public List<int> WriteRoleAcl { get; set; } = new();
    }
}
