using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using dig.API.Common;
using dig.API.Feature.Auth;

public class AzureAIService
{
    // sample url: "https://github.com/Azure-Samples/cognitive-services-REST-api-samples/raw/master/curl/form-recognizer/rest-api/invoice.pdf"

    public static async Task<Either<SimpleMessageError, Dictionary<string, Dictionary<string, object>>>> RecognizeInvoiceModel(string invoiceLink)
    {
        Console.WriteLine($"Azure AI Service Invoice Link: ${invoiceLink}");
        var key = Environment.GetEnvironmentVariable("FR_KEY");
        if (key is null)
        {
            return new Either<SimpleMessageError, Dictionary<string, Dictionary<string, object>>>(new SimpleMessageError("Failed to retrieve Azure AI key from ENV!"));
        }

        var endpoint = Environment.GetEnvironmentVariable("FR_ENDPOINT");
        if (endpoint is null)
        {
            return new Either<SimpleMessageError, Dictionary<string, Dictionary<string, object>>>(new SimpleMessageError("Failed to retrieve Azure AI endpoint from ENV!"));
        }

        AzureKeyCredential credential = new AzureKeyCredential(key);
        DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

        Dictionary<string, Dictionary<string, object>> invoiceFields = new Dictionary<string, Dictionary<string, object>>();

        // sample document document
        Uri invoiceUri = new Uri(invoiceLink);
        var modelName = Environment.GetEnvironmentVariable("FR_MODEL");
        if (modelName is null)
        {
            return new Either<SimpleMessageError, Dictionary<string, Dictionary<string, object>>>(new SimpleMessageError("Failed to retrieve Azure AI model name from ENV!"));
        }

        // TODO: check if replaced with correct form recognition model
        AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelName, invoiceUri); //prebuilt-invoice
        if (!operation.HasValue)
        {
            return new Either<SimpleMessageError, Dictionary<string, Dictionary<string, object>>>(new SimpleMessageError("Document analysis with Azure AI failed!"));
        }

        AnalyzeResult result = operation.Value;

        for (int i = 0; i < result.Documents.Count; i++)
        {
            Console.WriteLine($"Document {i}:");

            AnalyzedDocument document = result.Documents[i];

            // retrieving Invoice Date
            if (document.Fields.TryGetValue("InvoiceDate", out DocumentField invoiceDateField))
            {
                if (invoiceDateField.FieldType == DocumentFieldType.Date)
                {
                    string invoiceDate = invoiceDateField.Value.AsString();
                    Console.WriteLine($"Invoice date: '{invoiceDate}', with confidence {invoiceDateField.Confidence}");
                    
                    invoiceFields.Add("InvoiceDate", new Dictionary<string, object> {
                        { "Value", invoiceDate },
                        { "Confidence", invoiceDateField.Confidence }
                    });
                }
            }

            // retrieving Invoice Id
            if (document.Fields.TryGetValue("InvoiceId", out DocumentField invoiceIdField))
            {
                if (invoiceIdField.FieldType == DocumentFieldType.String)
                {
                    string invoiceId = invoiceIdField.Value.AsString();
                    Console.WriteLine($"Invoice id: '{invoiceId}', with confidence {invoiceIdField.Confidence}");
                    
                    invoiceFields.Add("InvoiceId", new Dictionary<string, object> {
                        { "Value", invoiceId },
                        { "Confidence", invoiceIdField.Confidence }
                    });
                }
            }

            // retrieving Total With No Tax
            if (document.Fields.TryGetValue("TotalWithNoTax", out DocumentField totalWithNoTaxField))
            {
                if (totalWithNoTaxField.FieldType == DocumentFieldType.Currency)
                {
                    string totalWithNoTax = totalWithNoTaxField.Value.AsString();
                    Console.WriteLine($"Total With No Tax: '{totalWithNoTax}', with confidence {totalWithNoTaxField.Confidence}");
                    
                    invoiceFields.Add("TotalWithNoTax", new Dictionary<string, object> {
                        { "Value", totalWithNoTax },
                        { "Confidence", totalWithNoTaxField.Confidence }
                    });
                }
            }

            // retrieving Total Tax
            if (document.Fields.TryGetValue("TotalTax", out DocumentField totalTaxField))
            {
                if (totalTaxField.FieldType == DocumentFieldType.Currency)
                {
                    string totalTax = totalTaxField.Value.AsString();
                    Console.WriteLine($"Total Tax: '{totalTax}', with confidence {totalTaxField.Confidence}");
                    
                    invoiceFields.Add("TotalTax", new Dictionary<string, object> {
                        { "Value", totalTax },
                        { "Confidence", totalTaxField.Confidence }
                    });
                }
            }

            // retrieving Total
            if (document.Fields.TryGetValue("Total", out DocumentField totalField))
            {
                if (totalField.FieldType == DocumentFieldType.Currency)
                {
                    string total = totalField.Value.AsString();
                    Console.WriteLine($"Total: '{total}', with confidence {totalField.Confidence}");
                    
                    invoiceFields.Add("Total", new Dictionary<string, object> {
                        { "Value", total },
                        { "Confidence", totalField.Confidence }
                    });
                }
            }

            // retrieving Vendor Address
            if (document.Fields.TryGetValue("VendorAddress", out DocumentField vendorAddressField))
            {
                if (vendorAddressField.FieldType == DocumentFieldType.String)
                {
                    string vendorAddress = vendorAddressField.Value.AsString();
                    Console.WriteLine($"Vendor Address: '{vendorAddress}', with confidence {vendorAddressField.Confidence}");
                    
                    invoiceFields.Add("VendorAddress", new Dictionary<string, object> {
                        { "Value", vendorAddress },
                        { "Confidence", vendorAddressField.Confidence }
                    });
                }
            }

            // retrieving Buyer Address
            if (document.Fields.TryGetValue("BuyerAddress", out DocumentField buyerAddressField))
            {
                if (buyerAddressField.FieldType == DocumentFieldType.String)
                {
                    string buyerAddress = buyerAddressField.Value.AsString();
                    Console.WriteLine($"Buyer Address: '{buyerAddress}', with confidence {buyerAddressField.Confidence}");
                    
                    invoiceFields.Add("BuyerAddress", new Dictionary<string, object> {
                        { "Value", buyerAddress },
                        { "Confidence", buyerAddressField.Confidence }
                    });
                }
            }

            // retrieving Date To Pay
            if (document.Fields.TryGetValue("DateToPay", out DocumentField dateToPayField))
            {
                if (dateToPayField.FieldType == DocumentFieldType.Date)
                {
                    string dateToPay = dateToPayField.Value.AsString();
                    Console.WriteLine($"Date To Pay: '{dateToPay}', with confidence {dateToPayField.Confidence}");
                    
                    invoiceFields.Add("DateToPay", new Dictionary<string, object> {
                        { "Value", dateToPay },
                        { "Confidence", dateToPayField.Confidence }
                    });
                }
            }

            // retrieving Vendor Reg Num
            if (document.Fields.TryGetValue("VendorRegNum", out DocumentField vendorRegNumField))
            {
                if (vendorRegNumField.FieldType == DocumentFieldType.String)
                {
                    string vendorRegNum = vendorRegNumField.Value.AsString();
                    Console.WriteLine($"Vendor Reg Num: '{vendorRegNum}', with confidence {vendorRegNumField.Confidence}");
                    
                    invoiceFields.Add("VendorRegNum", new Dictionary<string, object> {
                        { "Value", vendorRegNum },
                        { "Confidence", vendorRegNumField.Confidence }
                    });
                }
            }

            // retrieving Buyer Reg Num
            if (document.Fields.TryGetValue("BuyerRegNum", out DocumentField buyerRegNumField))
            {
                if (buyerRegNumField.FieldType == DocumentFieldType.String)
                {
                    string buyerRegNum = buyerRegNumField.Value.AsString();
                    Console.WriteLine($"Buyer Reg Num: '{buyerRegNum}', with confidence {buyerRegNumField.Confidence}");
                    
                    invoiceFields.Add("BuyerRegNum", new Dictionary<string, object> {
                        { "Value", buyerRegNum },
                        { "Confidence", buyerRegNumField.Confidence }
                    });
                }
            }

            // retrieving Vendor Company Name
            if (document.Fields.TryGetValue("VendorCompanyName", out DocumentField vendorCompanyNameField))
            {
                if (vendorCompanyNameField.FieldType == DocumentFieldType.String)
                {
                    string vendorCompanyName = vendorCompanyNameField.Value.AsString();
                    Console.WriteLine($"Vendor Company Name: '{vendorCompanyName}', with confidence {vendorCompanyNameField.Confidence}");
                    
                    invoiceFields.Add("VendorCompanyName", new Dictionary<string, object> {
                        { "Value", vendorCompanyName },
                        { "Confidence", vendorCompanyNameField.Confidence }
                    });
                }
            }

            // retrieving Buyer Company Name
            if (document.Fields.TryGetValue("BuyerCompanyName", out DocumentField buyerCompanyNameField))
            {
                if (buyerCompanyNameField.FieldType == DocumentFieldType.String)
                {
                    string buyerCompanyName = buyerCompanyNameField.Value.AsString();
                    Console.WriteLine($"Buyer Company Name: '{buyerCompanyName}', with confidence {buyerCompanyNameField.Confidence}");
                    
                    invoiceFields.Add("BuyerCompanyName", new Dictionary<string, object> {
                        { "Value", buyerCompanyName },
                        { "Confidence", buyerCompanyNameField.Confidence }
                    });
                }
            }

            // retrieving Vendor Bank Account
            if (document.Fields.TryGetValue("VendorBankAccount", out DocumentField vendorBankAccountField))
            {
                if (vendorBankAccountField.FieldType == DocumentFieldType.String)
                {
                    string vendorBankAccount = vendorBankAccountField.Value.AsString();
                    Console.WriteLine($"Vendor Bank Account: '{vendorBankAccount}', with confidence {vendorBankAccountField.Confidence}");
                    
                    invoiceFields.Add("VendorBankAccount", new Dictionary<string, object> {
                        { "Value", vendorBankAccount },
                        { "Confidence", vendorBankAccountField.Confidence }
                    });
                }
            }

            // retrieving Buyer Bank Account
            if (document.Fields.TryGetValue("BuyerBankAccount", out DocumentField buyerBankAccountField))
            {
                if (buyerBankAccountField.FieldType == DocumentFieldType.String)
                {
                    string buyerBankAccount = buyerBankAccountField.Value.AsString();
                    Console.WriteLine($"Buyer Bank Account: '{buyerBankAccount}', with confidence {buyerBankAccountField.Confidence}");
                    
                    invoiceFields.Add("BuyerBankAccount", new Dictionary<string, object> {
                        { "Value", buyerBankAccount },
                        { "Confidence", buyerBankAccountField.Confidence }
                    });
                }
            }

            // retrieving Vendor PVN num
            if (document.Fields.TryGetValue("VendorPVNnum", out DocumentField vendorPVNnumField))
            {
                if (vendorPVNnumField.FieldType == DocumentFieldType.String)
                {
                    string vendorPVNnum = vendorPVNnumField.Value.AsString();
                    Console.WriteLine($"Vendor PVN num: '{vendorPVNnum}', with confidence {vendorPVNnumField.Confidence}");
                    
                    invoiceFields.Add("VendorPVNnum", new Dictionary<string, object> {
                        { "Value", vendorPVNnum },
                        { "Confidence", vendorPVNnumField.Confidence }
                    });
                }
            }

            // retrieving Buyer PVN num
            if (document.Fields.TryGetValue("BuyerPVNnum", out DocumentField buyerPVNnumField))
            {
                if (buyerPVNnumField.FieldType == DocumentFieldType.String)
                {
                    string buyerPVNnum = buyerPVNnumField.Value.AsString();
                    Console.WriteLine($"Buyer PVN num: '{buyerPVNnum}', with confidence {buyerPVNnumField.Confidence}");
                    
                    invoiceFields.Add("BuyerPVNnum", new Dictionary<string, object> {
                        { "Value", buyerPVNnum },
                        { "Confidence", buyerPVNnumField.Confidence }
                    });
                }
            }

            // retrieving Physical Buyer Name
            if (document.Fields.TryGetValue("PhysicalBuyerName", out DocumentField physicalBuyerNameField))
            {
                if (physicalBuyerNameField.FieldType == DocumentFieldType.String)
                {
                    string physicalBuyerName = physicalBuyerNameField.Value.AsString();
                    Console.WriteLine($"Physical Buyer Name: '{physicalBuyerName}', with confidence {physicalBuyerNameField.Confidence}");
                    
                    invoiceFields.Add("PhysicalBuyerName", new Dictionary<string, object> {
                        { "Value", physicalBuyerName },
                        { "Confidence", physicalBuyerNameField.Confidence }
                    });
                }
            }
        }

        return new Either<SimpleMessageError, Dictionary<string, Dictionary<string, object>>>(invoiceFields);
    }
}