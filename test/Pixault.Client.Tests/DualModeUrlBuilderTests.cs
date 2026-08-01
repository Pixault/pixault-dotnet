using NUnit.Framework;
using Pixault.Client;

namespace Pixault.Client.Tests;

[TestFixture]
public class DualModeUrlBuilderTests
{
    private static PixaultUrlBuilder B(string id) => new("https://img.pixault.io", "tattoo", id);

    [Test] public void LegacyId_noTransform_original()
        => Assert.That(B("img_01JK").Build(), Is.EqualTo("https://img.pixault.io/tattoo/img_01JK/original.auto"));
    [Test] public void LegacyId_withTransform_transformLast()
        => Assert.That(B("img_01JK").Width(800).Format("webp").Build(),
            Is.EqualTo("https://img.pixault.io/tattoo/img_01JK/w_800.webp"));
    [Test] public void VidPrefix_isLegacy()
        => Assert.That(B("vid_9").Build(), Does.Contain("/tattoo/vid_9/original."));
    [Test] public void Slug_noTransform_publicIdGrammar()
        => Assert.That(B("sunset-beach").Build(), Is.EqualTo("https://img.pixault.io/tattoo/sunset-beach.auto"));
    [Test] public void Slug_withTransform_cloudinaryOrder()
        => Assert.That(B("sunset-beach").Width(800).Format("webp").Build(),
            Is.EqualTo("https://img.pixault.io/tattoo/w_800/sunset-beach.webp"));
}
