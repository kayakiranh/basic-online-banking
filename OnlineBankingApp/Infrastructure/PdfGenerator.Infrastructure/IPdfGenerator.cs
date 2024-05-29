namespace PdfGenerator.Infrastructure
{
    public interface IPdfService
    {
        string Generate(string accountNumber);
    }
}