using NUnit.Framework;
using Pixault.Client;

namespace Pixault.Client.Tests;

/// <summary>
/// A null publicId reaches the builder from data, not from a typo: a record with no image has a
/// null image id, and <c>string</c> rather than <c>string?</c> on the parameter is a compile-time
/// hint a nullable-oblivious caller never sees. On 2026-08-09 one such row threw
/// NullReferenceException out of <see cref="PixaultUrlBuilder.Build"/> and took all 309
/// barber.shop profile pages down with it.
///
/// A URL builder must not throw over missing data. Null is simply not a legacy id.
/// </summary>
[TestFixture]
public class NullPublicIdTests
{
    private static PixaultUrlBuilder B(string id) => new("https://img.pixault.io", "tattoo", id);

    [Test] public void NullPublicId_Build_doesNotThrow()
        => Assert.That(() => B(null!).Build(), Throws.Nothing);

    [Test] public void EmptyPublicId_Build_doesNotThrow()
        => Assert.That(() => B("").Build(), Throws.Nothing);

    [Test] public void NullPublicId_isNotTreatedAsLegacy()
        // Legacy grammar would put the id first and append "/original"; publicId grammar does not.
        => Assert.That(B(null!).Build(), Does.Not.Contain("/original."));

    [Test] public void NullPublicId_withTransform_doesNotThrow()
        => Assert.That(() => B(null!).Width(800).Format("webp").Build(), Throws.Nothing);

    [Test] public void NullPublicId_ToImgTag_doesNotThrow()
        // ToImgTag goes through VariantUrl, which branches on IsLegacyId separately from Build.
        => Assert.That(() => B(null!).ToImgTag("alt text", widths: new[] { 400 }), Throws.Nothing);

    [Test] public void NullPublicId_ToPictureTag_doesNotThrow()
        => Assert.That(() => B(null!).ToPictureTag("alt text", widths: new[] { 400 }), Throws.Nothing);
}
