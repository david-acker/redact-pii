using System.Net.Mime;
using RedactPii.Models;
using SkiaSharp;

namespace RedactPii.Services;

internal sealed class ImageRedactionService
{
    public ImageRedactionResult RedactImage(
        byte[] sourceImagesBytes,
        IEnumerable<SKRect> redactionAreas)
    {
        var bitmap = SKBitmap.Decode(sourceImagesBytes);
        using var canvas = new SKCanvas(bitmap);
        
        canvas.DrawBitmap(bitmap, 0, 0);
        
        var paint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill
        };

        foreach (var redactionArea in redactionAreas)
        {
            canvas.SaveLayer(redactionArea, paint);
            canvas.DrawRect(redactionArea, paint);
            canvas.Restore();
        }

        using var editedImage = SKImage.FromBitmap(bitmap);
        using var editedImageData = editedImage.Encode(SKEncodedImageFormat.Png, 100);

        return new ImageRedactionResult(editedImageData.ToArray(), MediaTypeNames.Image.Png);
    }
}