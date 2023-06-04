public class DocumentDto
{
    public Guid Id { get; set; }
    public string InvoiceId { get; set; }
    // public object Content { get; set; }
    public DocContentDto Content { get; set; }
    public string Link { get; set; }
    public string Status { get; set; }
    public long CreatedAt { get; set; }
}
