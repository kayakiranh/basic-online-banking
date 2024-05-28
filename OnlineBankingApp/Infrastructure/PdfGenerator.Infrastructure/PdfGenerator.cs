using System.Text.Json;

namespace PdfGenerator.Infrastructure
{
    [Serializable]
    public class PdfGenerator : IPdfGenerator
    {
        public string Generate(string accountNumber)
        {
            List<Tuple<DateTime, string, decimal>> accountTransactionHistory = new List<Tuple<DateTime, string, decimal>>();
            accountTransactionHistory.Add(new Tuple<DateTime, string, decimal>(DateTime.Now.AddDays(-30), "Bim", -100));
            accountTransactionHistory.Add(new Tuple<DateTime, string, decimal>(DateTime.Now.AddDays(-15), "Mehmet", +150));
            accountTransactionHistory.Add(new Tuple<DateTime, string, decimal>(DateTime.Now.AddDays(0), "Maaş", 1000));

            string pdfContent = JsonSerializer.Serialize(accountTransactionHistory);

            return $"~/files/pdf/{accountNumber}/{DateTime.Now.Date}/{Guid.NewGuid().ToString()}.pdf";
        }
    }
}