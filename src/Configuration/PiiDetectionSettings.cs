namespace RedactPii.Configuration;

public sealed class PiiDetectionSettings
{
    public const string ConfigurationPath = "PiiDetection";
    
    public bool UseOpenAiDetection { get; set; }
    
    public bool UseAzureDetection { get; set; }
}