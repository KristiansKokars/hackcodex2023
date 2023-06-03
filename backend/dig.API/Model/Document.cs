public class Document
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public Guid SubmitterId { get; set; }
    public Guid DocumentJobId { get; set; }
}