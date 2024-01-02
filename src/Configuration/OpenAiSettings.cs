using System.ComponentModel.DataAnnotations;

namespace RedactPii.Configuration;

public sealed class OpenAiSettings
{
    public const string ConfigurationPath = "OpenAi";
    
    [Required]
    public required string Key { get; set; }
}