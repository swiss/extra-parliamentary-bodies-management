using System.Text.RegularExpressions;

namespace Bk.APG.Business.Services;

public readonly record struct ReportExportSanitizationResult(string CleanText, bool HasOpenChanges);

public static partial class ReportExportHtmlSanitizer
{
    [GeneratedRegex("</?annotation\\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AnnotationTagRegex();

    [GeneratedRegex("data-flite-cid\\s*=", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ChangeTrackingMarkerRegex();

    public static ReportExportSanitizationResult Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return new ReportExportSanitizationResult(string.Empty, false);
        }

        var hasOpenChanges = ChangeTrackingMarkerRegex().IsMatch(html);

        var cleanText = AnnotationTagRegex().Replace(html, string.Empty);

        return new ReportExportSanitizationResult(cleanText, hasOpenChanges);
    }
}
