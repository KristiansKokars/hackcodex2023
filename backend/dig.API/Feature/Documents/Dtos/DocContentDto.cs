public class DocContentDto
{
    public DocFieldDto InvoiceDate { get; set; }
    public DocFieldDto TotalWithNoTax { get; set; }
    public DocFieldDto TotalTax { get; set; }
    public DocFieldDto Total { get; set; }
    public DocFieldDto VendorAddress { get; set; }
    public DocFieldDto BuyerAddress { get; set; }
    public DocFieldDto DateToPay { get; set; }
    public DocFieldDto VendorRegNum { get; set; }
    public DocFieldDto BuyerRegNum { get; set; }
    public DocFieldDto VendorCompanyName { get; set; }
    public DocFieldDto BuyerCompanyName { get; set; }
    public DocFieldDto VendorBankAccount { get; set; }
    public DocFieldDto BuyerBankAccount { get; set; }
    public DocFieldDto VendorPVNnum { get; set; }
    public DocFieldDto BuyerPVNnum { get; set; }
    public DocFieldDto PhysicalBuyerName { get; set; }
}

public class ResponseDocContentDto
{
    public object InvoiceDate { get; set; }
    public object TotalWithNoTax { get; set; }
    public object TotalTax { get; set; }
    public object Total { get; set; }
    public object VendorAddress { get; set; }
    public object BuyerAddress { get; set; }
    public object DateToPay { get; set; }
    public object VendorRegNum { get; set; }
    public object BuyerRegNum { get; set; }
    public object VendorCompanyName { get; set; }
    public object BuyerCompanyName { get; set; }
    public object VendorBankAccount { get; set; }
    public object BuyerBankAccount { get; set; }
    public object VendorPVNnum { get; set; }
    public object BuyerPVNnum { get; set; }
    public object PhysicalBuyerName { get; set; }
}

public class DocIdDto
{
    public Dictionary<string, object> InvoiceId { get; set; }
}

public class DocFieldDto
{
    public string Value { get; set; }
    public object Confidence { get; set; }
}