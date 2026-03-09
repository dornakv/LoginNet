namespace LoginNet.Contracts.DTOs
{
    public class NoteGetDTO
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public class NotePostDTO
    {
        public required string Title { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}
