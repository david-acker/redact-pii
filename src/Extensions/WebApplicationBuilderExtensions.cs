using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Managers;
using RedactPii.Configuration;
using RedactPii.Interfaces;
using RedactPii.Services;
using RedactPii.Services.Providers;

namespace RedactPii.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<PiiDetectionSettings>()
            .BindConfiguration(PiiDetectionSettings.ConfigurationPath);

        builder.Services.AddKeyedScoped<IPiiDetectionServiceProvider, AzurePiiDetectionServiceProvider>("azure");
        builder.Services.AddKeyedScoped<IPiiDetectionServiceProvider, OpenAiPiiDetectionServiceProvider>("openai");

        builder.Services.AddScoped<ImageRedactionService>();
        builder.Services.AddScoped<TextExtractionService>();
        builder.Services.AddScoped<PiiDetectionService>();
        
        builder.Services.AddScoped<PiiRedactionService>();

        return builder;
    }
    
    public static WebApplicationBuilder AddAzureDocumentIntelligenceServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<AzureDocumentIntelligenceSettings>()
            .BindConfiguration(AzureDocumentIntelligenceSettings.ConfigurationPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        builder.Services.AddScoped<DocumentAnalysisClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<AzureDocumentIntelligenceSettings>>().Value;
            
            var endpointUri = new Uri(settings.Endpoint);
            var credential = new AzureKeyCredential(settings.Key);

            return new DocumentAnalysisClient(endpointUri, credential);
        });

        return builder;
    }
    
    public static WebApplicationBuilder AddAzureLanguageServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<AzureLanguageSettings>()
            .BindConfiguration(AzureLanguageSettings.ConfigurationPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddScoped<TextAnalyticsClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<AzureLanguageSettings>>().Value;
            
            var endpointUri = new Uri(settings.Endpoint);
            var credential = new AzureKeyCredential(settings.Key);

            return new TextAnalyticsClient(endpointUri, credential);
        });

        return builder;
    }

    public static WebApplicationBuilder AddOpenAiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<OpenAiSettings>()
            .BindConfiguration(OpenAiSettings.ConfigurationPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddScoped<OpenAIService>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<OpenAiSettings>>().Value;

            var options = new OpenAiOptions
            {
                ApiKey = settings.Key
            };

            return new OpenAIService(options);
        });
        
        return builder;
    }
}