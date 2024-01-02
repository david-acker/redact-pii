using Azure.AI.TextAnalytics;
using RedactPii.Interfaces;

namespace RedactPii.Services.Providers;

internal sealed class AzurePiiDetectionServiceProvider : IPiiDetectionServiceProvider
{
    private readonly TextAnalyticsClient _textAnalyticsClient;
    
    public AzurePiiDetectionServiceProvider(TextAnalyticsClient textAnalyticsClient)
    {
        _textAnalyticsClient = textAnalyticsClient;
    }
    
    public async Task<IEnumerable<string>> DetectPii(
        string textContent,
        CancellationToken cancellationToken = default)
    {
        PiiEntityCollection response = 
            await _textAnalyticsClient.RecognizePiiEntitiesAsync(textContent, cancellationToken: cancellationToken);

        return response.Select(x => x.Text);
    }
}