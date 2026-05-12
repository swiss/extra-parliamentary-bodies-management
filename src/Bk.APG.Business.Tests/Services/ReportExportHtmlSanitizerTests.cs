using Bk.APG.Business.Services;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class ReportExportHtmlSanitizerTests
{
    [Test]
    public void Sanitize_WithAnnotationTag_KeepsInnerContentAndAllOtherMarkupVerbatim()
    {
        const string input = "<p><annotation class=\"lance-annotation-class\" data-lance-resolved=\"true\">Begruendung</annotation></p>";

        var result = ReportExportHtmlSanitizer.Sanitize(input);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.CleanText, Is.EqualTo("<p>Begruendung</p>"));
            Assert.That(result.HasOpenChanges, Is.False);
        }
    }

    [Test]
    public void Sanitize_WithChangeTrackingMarkers_FlagsOpenChangesAndKeepsInnerContent()
    {
        const string input = "<span data-flite-cid=\"2\">Foo Bar</span>";

        var result = ReportExportHtmlSanitizer.Sanitize(input);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.CleanText, Is.EqualTo("<span data-flite-cid=\"2\">Foo Bar</span>"));
            Assert.That(result.HasOpenChanges, Is.True);
        }
    }

    [Test]
    public void Sanitize_WithFullSampleHtml_OnlyStripsAnnotationTagsAndFlagsOpenChanges()
    {
        const string input = "<p><annotation class=\"lance-annotation-class\" data-lance-resolved=\"true\"><em>Das</em></annotation> " +
                             "<span data-flite-cid=\"2\" class=\"ice-del\">sind</span>" +
                             "<span data-flite-cid=\"2\" class=\"ice-ins\">ist</span> " +
                             "<strong>eine</strong> " +
                             "<annotation data-lance-resolved=\"false\">Begr\u00fcndung</annotation>" +
                             "<span class=\"ice-ins\">&nbsp;mit Zusatz</span>.</p>";

        const string expected = "<p><em>Das</em> " +
                                "<span data-flite-cid=\"2\" class=\"ice-del\">sind</span>" +
                                "<span data-flite-cid=\"2\" class=\"ice-ins\">ist</span> " +
                                "<strong>eine</strong> " +
                                "Begr\u00fcndung" +
                                "<span class=\"ice-ins\">&nbsp;mit Zusatz</span>.</p>";

        var result = ReportExportHtmlSanitizer.Sanitize(input);

        Assert.Multiple(() =>
        {
            Assert.That(result.CleanText, expression: Is.EqualTo(expected));
            Assert.That(result.CleanText, Does.Not.Contain("<annotation"));
            Assert.That(result.HasOpenChanges, Is.True);
        });
    }

    [Test]
    public void Sanitize_NullOrWhitespace_ReturnsEmptyWithNoChanges()
    {
        Assert.Multiple(() =>
        {
            var r1 = ReportExportHtmlSanitizer.Sanitize(null);
            Assert.That(r1.CleanText, Is.Empty);
            Assert.That(r1.HasOpenChanges, Is.False);

            var r2 = ReportExportHtmlSanitizer.Sanitize("   ");
            Assert.That(r2.CleanText, Is.Empty);
            Assert.That(r2.HasOpenChanges, Is.False);
        });
    }
}
