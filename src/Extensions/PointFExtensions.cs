using SkiaSharp;

namespace RedactPii.Extensions;

internal static class PointFExtensions
{
    public static SKRect ConvertToSkRect(this IReadOnlyList<System.Drawing.PointF> boundingPolygon)
    {
        if (boundingPolygon.Count < 4)
        {
            throw new ArgumentException("Bounding polygon must have at least 4 points.");
        }
        
        var startX = boundingPolygon.Min(bp => bp.X);
        var startY = boundingPolygon.Min(bp => bp.Y);
            
        var endX = boundingPolygon.Max(bp => bp.X);
        var endY = boundingPolygon.Max(bp => bp.Y);
            
        return new SKRect(startX, startY, endX, endY);
    }
}