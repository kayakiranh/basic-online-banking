namespace PdfGenerator.Infrastructure
{
    public interface IPdfGenerator
    {
        string Generate(string accountNumber);
    }
}
