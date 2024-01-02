using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Options;
using RedactPii.Configuration;
using RedactPii.Interfaces;

namespace RedactPii.Services;

internal sealed class PiiDetectionService
{
    private readonly PiiDetectionSettings _piiDetectionSettings;
    private readonly IPiiDetectionServiceProvider _azurePiiDetectionServiceProvider;
    private readonly IPiiDetectionServiceProvider _openAiPiiDetectionServiceProvider;

    public PiiDetectionService(
        IOptions<PiiDetectionSettings> piiDetectionSettingsOptions,
        [FromKeyedServices("azure")] IPiiDetectionServiceProvider azurePiiDetectionServiceProvider,
        [FromKeyedServices("openai")] IPiiDetectionServiceProvider openAiPiiDetectionServiceProvider)
    {
        _piiDetectionSettings = piiDetectionSettingsOptions.Value;
        _azurePiiDetectionServiceProvider = azurePiiDetectionServiceProvider;
        _openAiPiiDetectionServiceProvider = openAiPiiDetectionServiceProvider;
    }

    public async Task<ISet<string>> ExtractPii(string textContent, CancellationToken cancellationToken = default)
    {
        var piiDetectionTasks = new List<Task<IEnumerable<string>>>();

        if (_piiDetectionSettings.UseAzureDetection)
        {
            piiDetectionTasks.Add(_azurePiiDetectionServiceProvider.DetectPii(textContent, cancellationToken));
        }

        if (_piiDetectionSettings.UseOpenAiDetection)
        {
            piiDetectionTasks.Add(_openAiPiiDetectionServiceProvider.DetectPii(textContent, cancellationToken));
        }

        var piiDetectionResults = await Task.WhenAll(piiDetectionTasks);

        return piiDetectionResults.SelectMany(x => x).ToHashSet();
    }

    public IEnumerable<DocumentWord> GetWordsContainingPii(
        IList<DocumentWord> extractedWords,
        IEnumerable<string> piiValues)
    {
        var allWordsContainingPii = new HashSet<DocumentWord>();
        
        foreach (var piiValue in piiValues)
        {
            // Naive solution to ensure that multi-word PII values are redacted properly.
            // e.g. "John Doe" will span multiple words ["John", "Doe"] extracted from the document.
            var piiValueComponents = piiValue
                .Trim()
                .Split(" ")
                .Select(x => x.Trim());

            var wordsContainingPii = extractedWords
                .Where(x => piiValueComponents.Any(y => x.Content.Contains(y)));
            
            allWordsContainingPii.UnionWith(wordsContainingPii);
        }

        return allWordsContainingPii;
    }
}