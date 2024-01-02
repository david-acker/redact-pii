using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using RedactPii.Interfaces;

namespace RedactPii.Services.Providers;

internal sealed class OpenAiPiiDetectionServiceProvider : IPiiDetectionServiceProvider
{
    private readonly OpenAIService _openAiClient;
    
    public OpenAiPiiDetectionServiceProvider(OpenAIService openAiClient)
    {
        _openAiClient = openAiClient;
    }

    public async Task<IEnumerable<string>> DetectPii(
        string textContent,
        CancellationToken cancellationToken = default)
    {
        var userMessage = $"""
                           You are detecting personally identifiable information (PII) in the provided text.
                           List each token or group of tokens in the text that may contain PII (for example: credit card numbers, security codes, names, addresses).
                           Do not modify or change the text in any way, or add labels.
                           Exclude labels, descriptive text, other text elements which may refer to or label PII, but are not actually PII themselves (for example: "Card number", "Expiration", "Country").
                           Also exclude text artifacts, incorrectly extracted text, or miscellaneous text that is unrelated to the PII.
                           Display each piece of PII as-is with no additional quotes, symbols, or other characters:
                           
                           {textContent}
                           """;

        var request = new ChatCompletionCreateRequest
        {
            Messages = [ChatMessage.FromUser(userMessage)],
            Model = OpenAI.ObjectModels.Models.Gpt_4,
            Temperature = 0,
            N = 1,
            MaxTokens = 512
        };

        var completionResult = 
            await _openAiClient.ChatCompletion.CreateCompletion(
                request,
                cancellationToken: cancellationToken);

        if (!completionResult.Successful)
        {
            var errorMessage = completionResult.Error?.Message ?? "An unknown error occurred";
            throw new Exception($"Failed to get response from OpenAI: {errorMessage}");
        }

        var response = completionResult.Choices.FirstOrDefault()?.Message.Content;
        if (response == null)
        {
            throw new Exception($"Failed to get response from OpenAI: Chat completion choices empty");
        }
        
        return response.Trim()
            .Split(Environment.NewLine)
            .Select(x => x.Trim());
    }
}