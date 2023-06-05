public class DocumentDto
{
    public Guid Id { get; set; }
    public string InvoiceId { get; set; }
    public ResponseDocContentDto Content { get; set; } //DocContentDto
    public string Link { get; set; }
    public string Status { get; set; }
    public long CreatedAt { get; set; }
    public double MinAllowedPercent { get; set; }
}
