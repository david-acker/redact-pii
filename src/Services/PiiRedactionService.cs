using RedactPii.Extensions;
using RedactPii.Models;
using SkiaSharp;

namespace RedactPii.Services;

internal sealed class PiiRedactionService
{
    private readonly PiiDetectionService _piiDetectionService;
    private readonly TextExtractionService _textExtractionService;
    private readonly ImageRedactionService _imageRedactionService;
    
    public PiiRedactionService(
        PiiDetectionService piiDetectionService,
        TextExtractionService textExtractionService,
        ImageRedactionService imageRedactionService)
    {
        _piiDetectionService = piiDetectionService;
        _textExtractionService = textExtractionService;
        _imageRedactionService = imageRedactionService;
    }
    
    public async Task<ImageRedactionResult> RedactPii(
        byte[] sourceImageBytes, 
        CancellationToken cancellationToken = default)
    {
        var documentAnalysisResult = await _textExtractionService.ExtractTextFromImage(sourceImageBytes, cancellationToken);
        
        var piiValues = await _piiDetectionService.ExtractPii(documentAnalysisResult.Content, cancellationToken);

        var redactionAreas = Enumerable.Empty<SKRect>();
        if (piiValues.Count > 0)
        {
            var extractedWords = documentAnalysisResult.Pages
                .SelectMany(x => x.Words)
                .ToList();

            var extractedWordsContainingPii = _piiDetectionService.GetWordsContainingPii(extractedWords, piiValues);

            redactionAreas = extractedWordsContainingPii
                .Select(word => word.BoundingPolygon.ConvertToSkRect())
                .ToHashSet();
        }
        
        return _imageRedactionService.RedactImage(sourceImageBytes, redactionAreas);
    }
}