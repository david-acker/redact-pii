using RedactPii.Extensions;
using RedactPii.Services;

var builder = WebApplication.CreateBuilder(args)
    .AddAzureDocumentIntelligenceServices()
    .AddAzureLanguageServices()
    .AddOpenAiServices()
    .AddApplicationServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/redact-pii", async (IFormFile file, PiiRedactionService redactPiiService, CancellationToken cancellationToken) =>
{
    await using var fileStream = file.OpenReadStream(); 
    
    using var sourceImageStream = new MemoryStream();
    await fileStream.CopyToAsync(sourceImageStream, cancellationToken);
    var sourceImageBytes = sourceImageStream.ToArray();
    
    var (redactedImageContent, redactedImageContentType) = 
        await redactPiiService.RedactPii(sourceImageBytes, cancellationToken);

    return Results.File(
        redactedImageContent,
        contentType: redactedImageContentType,
        fileDownloadName: file.FileName);
})
// For testing purposes only
.DisableAntiforgery();

app.Run();