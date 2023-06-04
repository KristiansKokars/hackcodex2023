using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using dig.API.Common;
using dig.API.Feature.Auth;

public class AzureAIService
{
    // sample url: "https://github.com/Azure-Samples/cognitive-services-REST-api-samples/raw/master/curl/form-recognizer/rest-api/invoice.pdf"

    public static async Task<Dictionary<string, Dictionary<string, object>>> RecognizeInvoiceModel(string invoiceLink)
    {
        Console.WriteLine($"Azure AI Service Invoice Link: ${invoiceLink}");
        var key = Environment.GetEnvironmentVariable("FR_KEY");
        if (key is null)
        {
            throw new Exception("Failed to retrieve Azure AI key from ENV!");
        }

        var endpoint = Environment.GetEnvironmentVariable("FR_ENDPOINT");
        if (endpoint is null)
        {
            throw new Exception("Failed to retrieve Azure AI endpoint from ENV!");
        }

        AzureKeyCredential credential = new AzureKeyCredential(key);
        DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

        Dictionary<string, Dictionary<string, object>> invoiceFields = new Dictionary<string, Dictionary<string, object>>();

        // sample document document
        Uri invoiceUri = new Uri(invoiceLink);
        var modelName = Environment.GetEnvironmentVariable("FR_MODEL");
        if (modelName is null)
        {
            throw new Exception("Failed to retrieve Azure AI model name from ENV!");
        }

        // TODO: check if replaced with correct form recognition model
        AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelName, invoiceUri); //prebuilt-invoice
        if (!operation.HasValue)
        {
            throw new Exception("Document analysis with Azure AI failed!");
        }

        AnalyzeResult result = operation.Value;

        // Prepare .labels.json file
        // TODO: https://learn.microsoft.com/en-us/dotnet/api/azure.ai.formrecognizer.documentanalysis.analyzeresult?view=azure-dotnet
        // take a look into Documents properties

        // Read document contents
        Console.WriteLine("\n Document analysis result: \n");
        Console.WriteLine(result.KeyValuePairs);
        Console.WriteLine();

        for (int i = 0; i < result.Documents.Count; i++)
        {
            Console.WriteLine($"Document {i}:");

            AnalyzedDocument document = result.Documents[i];

            // retrieving Invoice Date
            if (document.Fields.TryGetValue("InvoiceDate", out DocumentField invoiceDateField))
            {
                // TODO: remove in prod
                Console.WriteLine(invoiceDateField.FieldType.ToString());

                if (invoiceDateField.FieldType != DocumentFieldType.Unknown)
                {
                    var invoiceDate = invoiceDateField.Content; // Value.AsString()
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
                // TODO: remove in prod
                Console.WriteLine(invoiceIdField.FieldType.ToString());

                if (invoiceIdField.FieldType != DocumentFieldType.Unknown)
                {
                    string invoiceId = invoiceIdField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(totalWithNoTaxField.FieldType.ToString());

                if (totalWithNoTaxField.FieldType != DocumentFieldType.Unknown)
                {
                    string totalWithNoTax = totalWithNoTaxField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(totalTaxField.FieldType.ToString());

                if (totalTaxField.FieldType != DocumentFieldType.Unknown)
                {
                    string totalTax = totalTaxField.Content;
                    Console.WriteLine($"Total Tax: '{totalTax}', with confid+ence {totalTaxField.Confidence}");
                    
                    invoiceFields.Add("TotalTax", new Dictionary<string, object> {
                        { "Value", totalTax },
                        { "Confidence", totalTaxField.Confidence }
                    });
                }
            }

            // retrieving Total
            if (document.Fields.TryGetValue("Total", out DocumentField totalField))
            {
                // TODO: remove in prod
                Console.WriteLine(totalField.FieldType.ToString());

                if (totalField.FieldType != DocumentFieldType.Unknown)
                {
                    string total = totalField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(vendorAddressField.FieldType.ToString());

                if (vendorAddressField.FieldType != DocumentFieldType.Unknown)
                {
                    string vendorAddress = vendorAddressField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(buyerAddressField.FieldType.ToString());

                if (buyerAddressField.FieldType != DocumentFieldType.Unknown)
                {
                    string buyerAddress = buyerAddressField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(dateToPayField.FieldType.ToString());

                if (dateToPayField.FieldType != DocumentFieldType.Unknown)
                {
                    var dateToPay = dateToPayField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(vendorRegNumField.FieldType.ToString());

                if (vendorRegNumField.FieldType != DocumentFieldType.Unknown)
                {
                    string vendorRegNum = vendorRegNumField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(buyerRegNumField.FieldType.ToString());

                if (buyerRegNumField.FieldType != DocumentFieldType.Unknown)
                {
                    string buyerRegNum = buyerRegNumField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(vendorCompanyNameField.FieldType.ToString());

                if (vendorCompanyNameField.FieldType != DocumentFieldType.Unknown)
                {
                    string vendorCompanyName = vendorCompanyNameField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(buyerCompanyNameField.FieldType.ToString());

                if (buyerCompanyNameField.FieldType != DocumentFieldType.Unknown)
                {
                    string buyerCompanyName = buyerCompanyNameField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(vendorBankAccountField.FieldType.ToString());

                if (vendorBankAccountField.FieldType != DocumentFieldType.Unknown)
                {
                    string vendorBankAccount = vendorBankAccountField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(buyerBankAccountField.FieldType.ToString());

                if (buyerBankAccountField.FieldType != DocumentFieldType.Unknown)
                {
                    string buyerBankAccount = buyerBankAccountField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(vendorPVNnumField.FieldType.ToString());
                
                if (vendorPVNnumField.FieldType != DocumentFieldType.Unknown)
                {
                    string vendorPVNnum = vendorPVNnumField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(buyerPVNnumField.FieldType.ToString());

                if (buyerPVNnumField.FieldType != DocumentFieldType.Unknown)
                {
                    string buyerPVNnum = buyerPVNnumField.Content;
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
                // TODO: remove in prod
                Console.WriteLine(physicalBuyerNameField.FieldType.ToString());

                if (physicalBuyerNameField.FieldType != DocumentFieldType.Unknown)
                {
                    string physicalBuyerName = physicalBuyerNameField.Content;
                    Console.WriteLine($"Physical Buyer Name: '{physicalBuyerName}', with confidence {physicalBuyerNameField.Confidence}");
                    
                    invoiceFields.Add("PhysicalBuyerName", new Dictionary<string, object> {
                        { "Value", physicalBuyerName },
                        { "Confidence", physicalBuyerNameField.Confidence }
                    });
                }
            }
        }

        return invoiceFields;
    }
}