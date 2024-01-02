using System.ComponentModel.DataAnnotations;

namespace RedactPii.Configuration;

internal sealed class AzureLanguageSettings
{    
    public const string ConfigurationPath = "Azure:Language";
    
    [Required, Url]
    public required string Endpoint { get; set; }
    
    [Required]
    public required string Key { get; set; }
}