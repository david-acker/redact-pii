using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace RedactPii.Services;

internal sealed class TextExtractionService
{
    private readonly DocumentAnalysisClient _documentAnalysisClient;
    
    public TextExtractionService(DocumentAnalysisClient documentAnalysisClient)
    {
        _documentAnalysisClient = documentAnalysisClient;
    }
    
    public async Task<AnalyzeResult> ExtractTextFromImage(
        byte[] sourceImageBytes,
        CancellationToken cancellationToken)
    {
        using var sourceImageStream = new MemoryStream(sourceImageBytes);
        
        var documentAnalysisOperation = await _documentAnalysisClient.AnalyzeDocumentAsync(
            WaitUntil.Completed, 
            "prebuilt-read", 
            document: sourceImageStream,
            cancellationToken: cancellationToken);

        return documentAnalysisOperation.Value;
    }
}