using System.ComponentModel.DataAnnotations;

namespace RedactPii.Configuration;

internal sealed class AzureDocumentIntelligenceSettings
{
    public const string ConfigurationPath = "Azure:DocumentIntelligence";
    
    [Required, Url]
    public required string Endpoint { get; set; }
    
    [Required]
    public required string Key { get; set; }
}