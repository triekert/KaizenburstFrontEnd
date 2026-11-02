using System;
using System.Threading.Tasks;

namespace Fasetto.Word.Web.Server.Data.GoogleApiInvoiceReader
{
    // ✅ CORRECT: Code placed inside a method
    public class BatchProgram
    {
        public static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            var processor = new InvoiceProcessorService(apiKey);

            var imageFiles = new[]
            {
            @"C:\Invoices\Invoice1.jpg",
            @"C:\Invoices\Receipt2.png"
        };

            await processor.BatchProcessImagesToCsvAsync(imageFiles, @"C:\Invoices\CSV_Outputs");
        }
    }

}
