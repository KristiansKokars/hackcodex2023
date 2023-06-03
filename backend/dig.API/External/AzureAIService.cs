using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using System.Text.Json;

public class AzureAIService
{
    // "https://github.com/Azure-Samples/cognitive-services-REST-api-samples/raw/master/curl/form-recognizer/rest-api/invoice.pdf"
    public static async Task<Dictionary<string, object>> RecognizeInvoiceModel(string invoiceLink)
    {
        Console.WriteLine($"Azure AI Service Invoice Link: ${invoiceLink}");
        //use your `key` and `endpoint` environment variables to create your `AzureKeyCredential` and `DocumentAnalysisClient` instances
        string key = Environment.GetEnvironmentVariable("FR_KEY");
        string endpoint = Environment.GetEnvironmentVariable("FR_ENDPOINT");
        AzureKeyCredential credential = new AzureKeyCredential(key);
        DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

        Dictionary<string, object> invoiceFields = new Dictionary<string, object>();

        // sample document document
        Uri invoiceUri = new Uri(invoiceLink);

        AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-invoice", invoiceUri);

        AnalyzeResult result = operation.Value;

        for (int i = 0; i < result.Documents.Count; i++)
        {
            Console.WriteLine($"Document {i}:");

            AnalyzedDocument document = result.Documents[i];

            if (document.Fields.TryGetValue("VendorName", out DocumentField vendorNameField))
            {
                if (vendorNameField.FieldType == DocumentFieldType.String)
                {
                    string vendorName = vendorNameField.Value.AsString();
                    Console.WriteLine($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}");
                    
                    invoiceFields.Add("vendor_name", vendorName);
                }
            }

            if (document.Fields.TryGetValue("CustomerName", out DocumentField customerNameField))
            {
                if (customerNameField.FieldType == DocumentFieldType.String)
                {
                    string customerName = customerNameField.Value.AsString();
                    Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");

                    invoiceFields.Add("customer_name", customerName);
                }
            }

            if (document.Fields.TryGetValue("Items", out DocumentField itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {
                    var itemsList = "[";

                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        Console.WriteLine("Item:");

                        Dictionary<string, object> item = new Dictionary<string, object>();

                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                            if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                            {
                                if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                {
                                    string itemDescription = itemDescriptionField.Value.AsString();

                                    Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");

                                    item.Add("description", itemDescription);
                                }
                            }

                            if (itemFields.TryGetValue("Amount", out DocumentField itemAmountField))
                            {
                                if (itemAmountField.FieldType == DocumentFieldType.Currency)
                                {
                                    CurrencyValue itemAmount = itemAmountField.Value.AsCurrency();

                                    Console.WriteLine($"  Amount: '{itemAmount.Symbol}{itemAmount.Amount}', with confidence {itemAmountField.Confidence}");

                                    item.Add("amount", itemAmount);
                                }
                            }
                        }

                        var itemJson = JsonSerializer.Serialize(item);
                        itemsList += itemJson;
                    }

                    itemsList += "]";
                    invoiceFields.Add("items", itemsList);
                }
            }

            if (document.Fields.TryGetValue("SubTotal", out DocumentField subTotalField))
            {
                if (subTotalField.FieldType == DocumentFieldType.Currency)
                {
                    CurrencyValue subTotal = subTotalField.Value.AsCurrency();
                    Console.WriteLine($"Sub Total: '{subTotal.Symbol}{subTotal.Amount}', with confidence {subTotalField.Confidence}");

                    invoiceFields.Add("sub_total", subTotal);
                }
            }

            if (document.Fields.TryGetValue("TotalTax", out DocumentField totalTaxField))
            {
                if (totalTaxField.FieldType == DocumentFieldType.Currency)
                {
                    CurrencyValue totalTax = totalTaxField.Value.AsCurrency();
                    Console.WriteLine($"Total Tax: '{totalTax.Symbol}{totalTax.Amount}', with confidence {totalTaxField.Confidence}");

                    invoiceFields.Add("total_tax", totalTax);
                }
            }

            if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField invoiceTotalField))
            {
                if (invoiceTotalField.FieldType == DocumentFieldType.Currency)
                {
                    CurrencyValue invoiceTotal = invoiceTotalField.Value.AsCurrency();
                    Console.WriteLine($"Invoice Total: '{invoiceTotal.Symbol}{invoiceTotal.Amount}', with confidence {invoiceTotalField.Confidence}");

                    invoiceFields.Add("invoice_total", invoiceTotal);
                }
            }
        }

        return invoiceFields;
    }
}