public class Document
{
    public Guid Id { get; set; }
    public string InvoiceId { get; set; }
    public string Content { get; set; }
    public string Link { get; set; }
    public Guid UserId { get; set; }

    // Correct, Faulty
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}