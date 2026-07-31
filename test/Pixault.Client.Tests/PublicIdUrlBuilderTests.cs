using NUnit.Framework;
using Pixault.Client;

namespace Pixault.Client.Tests;

[TestFixture]
public class PublicIdUrlBuilderTests
{
    private static PixaultUrlBuilder B() =>
        new("https://img.pixault.io", "tattoo", "sunset-beach");

    [Test]
    public void NoTransform_defaultsToAuto()
        => Assert.That(B().Build(), Is.EqualTo("https://img.pixault.io/tattoo/sunset-beach.auto"));

    [Test]
    public void ExplicitFormat_noTransform()
        => Assert.That(B().Format("webp").Build(), Is.EqualTo("https://img.pixault.io/tattoo/sunset-beach.webp"));

    [Test]
    public void Transform_cloudinaryOrder_publicIdLast()
        => Assert.That(B().Width(800).Format("webp").Build(),
            Is.EqualTo("https://img.pixault.io/tattoo/w_800/sunset-beach.webp"));

    [Test]
    public void MultiParam_and_named_transform_order()
        => Assert.That(B().Transform("gallery").Width(800).Quality(85).Build(),
            Is.EqualTo("https://img.pixault.io/tattoo/t_gallery,w_800,q_85/sunset-beach.auto"));
}
