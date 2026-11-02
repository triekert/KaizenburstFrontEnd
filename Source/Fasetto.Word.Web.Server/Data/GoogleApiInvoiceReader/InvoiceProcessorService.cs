namespace Fasetto.Word.Web.Server.Data.GoogleApiInvoiceReader
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CsvHelper;
    using Google.GenAI;
    using Google.GenAI.Types;

    // Define the alias at the top of the file
    using GenAiType = Google.GenAI.Types.Type;
    using File = System.IO.File;

    public class InvoiceProcessorService
    {
        private readonly Client m_geminiClient;

        public InvoiceProcessorService(string apiKey)
        {
            // Initialize official Google GenAI Client
            m_geminiClient = new Client(apiKey: apiKey);
        }

        public async Task BatchProcessImagesToCsvAsync(IEnumerable<string> imagePaths, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);

            foreach (var imagePath in imagePaths)
            {
                try
                {
                    // 1. Read image bytes and infer MIME type
                    var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                    var mimeType = GetMimeType(imagePath);

                    // 2. Query Gemini for structured extraction
                    var extractedData = await ExtractInvoiceDataAsync(imageBytes, mimeType);

                    // 3. Convert extracted result to CSV
                    var csvFileName = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(imagePath)}.csv");
                    ExportToCsv(extractedData.Items, csvFileName);

                    Console.WriteLine($"Successfully converted {imagePath} -> {csvFileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process {imagePath}: {ex.Message}");
                }
            }
        }

        private async Task<InvoiceExtractionResult> ExtractInvoiceDataAsync(byte[] imageBytes, string mimeType)
        {
            var prompt = "Extract all line items, promotional discounts, quantities, unit prices, and total amounts from this receipt or invoice.";

                    // 1. Build the multimodal content parts
                    var content = new Content
                    {
                        Parts = new List<Part>
            {
                new Part { Text = prompt },
                new Part
                {
                    InlineData = new Blob
                    {
                        MimeType = mimeType,
                        Data = imageBytes // Pass byte[] directly
                    }
                }
            }
                    };

                    // 2. Define the response schema and generation config
                    var config = new GenerateContentConfig
                    {
                        // Set response format directly on GenerateContentConfig
                        ResponseMimeType = "application/json",


                    ResponseSchema = new Schema
                    {
                        Type = GenAiType.Object,
                        Properties = new Dictionary<string, Schema>
                        {
                            ["items"] = new Schema
                            {
                                Type = GenAiType.Array,
                                Items = new Schema
                                {
                                    Type = GenAiType.Object,
                                    Properties = new Dictionary<string, Schema>
                                    {
                                        ["item"] = new Schema { Type = GenAiType.String },
                                        ["quantity"] = new Schema { Type = GenAiType.Number },
                                        ["unitPrice"] = new Schema { Type = GenAiType.Number },
                                        ["amount"] = new Schema { Type = GenAiType.Number }
                                    },
                                    Required = new List<string> { "item", "quantity", "amount" }
                                }
                            }
                        }
                    }
                    };
                    // 3. Call GenerateContentAsync directly with model, contents, and config
                    var response = await m_geminiClient.Models.GenerateContentAsync(
                model: "gemini-2.5-flash",
                contents: new List<Content> { content },
                config: config
            );

            // 4. Deserialize JSON result
            return JsonSerializer.Deserialize<InvoiceExtractionResult>(response.Text) 
                   ?? new InvoiceExtractionResult();
        }
        private void ExportToCsv(List<InvoiceLineItem> items, string outputPath)
        {
            using var writer = new StreamWriter(outputPath);
            using var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
            csv.WriteRecords(items);
        }
        private string GetMimeType(string filePath)
        {
            return Path.GetExtension(filePath).ToLower() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                _ => "image/jpeg"
            };
        }
    }
}
