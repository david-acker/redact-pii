namespace RedactPii.Interfaces;

internal interface IPiiDetectionServiceProvider
{
    Task<IEnumerable<string>> DetectPii(string textContent, CancellationToken cancellationToken = default);
}