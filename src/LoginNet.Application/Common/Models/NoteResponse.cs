namespace LoginNet.Application.Common.Models
{
    public class NoteResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
