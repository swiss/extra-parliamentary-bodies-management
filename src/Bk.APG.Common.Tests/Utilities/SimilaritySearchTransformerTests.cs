using Bk.APG.Common.Utilities;
using NUnit.Framework;

namespace Bk.APG.Common.Tests.Utilities;

[TestFixture]
internal class SimilaritySearchTransformerTests
{
    [TestFixture]
    internal class StringUtilsTest
    {
        [TestCase("Uelker", "ulker")]
        [TestCase("Hoerner", "horner")]
        [TestCase("Laeţiťia", "latitia")]
        [TestCase("Bär", "bar")]
        [TestCase("ágnes", "agnes")]
        [TestCase("àgneš", "agnes")]
        [TestCase("âgneş", "agnes")]
        [TestCase("ãgneś", "agnes")]
        [TestCase("ågnes", "agnes")]
        [TestCase("ægnes", "agnes")]
        [TestCase("āgnes", "agnes")]
        [TestCase("ągnes", "agnes")]
        [TestCase("ăgnes", "agnes")]
        [TestCase("çakin", "cakin")]
        [TestCase("čakín", "cakin")]
        [TestCase("ċakìn", "cakin")]
        [TestCase("ćakîn", "cakin")]
        [TestCase("ðurđa", "durda")]
        [TestCase("đurđa", "durda")]
        [TestCase("ďurđa", "durda")]
        [TestCase("údó", "udo")]
        [TestCase("ùdo", "udo")]
        [TestCase("réka", "reka")]
        [TestCase("rèka", "reka")]
        [TestCase("rêka", "reka")]
        [TestCase("rėka", "reka")]
        [TestCase("ręka", "reka")]
        [TestCase("rěka", "reka")]
        [TestCase("rěķa", "reka")]
        [TestCase("ģerħardd", "gerhard")]
        [TestCase("ġerħard", "gerhard")]
        [TestCase("ğerħard", "gerhard")]
        [TestCase("ŀeoñard", "leonard")]
        [TestCase("ļeoņard", "leonard")]
        [TestCase("łeońard", "leonard")]
        [TestCase("ľeonard", "leonard")]
        [TestCase("ĺeoňard", "leonard")]
        [TestCase("ödön", "odon")]
        [TestCase("ottó", "oto")]
        [TestCase("otþò", "oto")]
        [TestCase("enikő", "eniko")]
        [TestCase("gýőžike", "giozike")]
        [TestCase("gÿõżike", "giozike")]
        [TestCase("gyôźike", "giozike")]
        [TestCase("enïkø", "eniko")]
        [TestCase("enĩkœ", "eniko")]
        [TestCase("enīkō", "eniko")]
        [TestCase("enįkő", "eniko")]
        [TestCase("enıkő", "eniko")]
        [TestCase("ŗobert", "robert")]
        [TestCase("ŕobert", "robert")]
        [TestCase("řobert", "robert")]
        [TestCase("müller", "muler")]
        [TestCase("mûller", "muler")]
        [TestCase("mūller", "muler")]
        [TestCase("mųller", "muler")]
        [TestCase("můller", "muler")]
        [TestCase("műller", "muler")]
        [TestCase("'otto'", "oto")]
        [TestCase("'otto-", "oto ")]
        public void Reduce_WhenCalled_Succeeds(string input, string expected)
        {
            var reducedText = SimilaritySearchTransformer.Reduce(input);

            Assert.That(reducedText, Is.EqualTo(expected));
        }
    }
}
