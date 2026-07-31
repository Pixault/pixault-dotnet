using System.Net;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Pixault.Client;

namespace Pixault.Client.Tests;

[TestFixture]
public class UploadClientFieldsTests
{
    private sealed class CapturingHandler : HttpMessageHandler
    {
        public string? Body;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken ct)
        {
            Body = await request.Content!.ReadAsStringAsync(ct);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"imageId\":\"img_x\",\"url\":\"u\",\"publicId\":\"sunset\"}",
                    System.Text.Encoding.UTF8, "application/json")
            };
        }
    }

    [Test]
    public async Task UploadAsync_sendsNameAndOverwrite_andReadsPublicId()
    {
        var handler = new CapturingHandler();
        var http = new HttpClient(handler) { BaseAddress = new System.Uri("https://api.pixault.io") };
        var client = new PixaultUploadClient(http, Options.Create(new PixaultOptions()));

        using var data = new System.IO.MemoryStream(new byte[] { 1, 2, 3 });
        var resp = await client.UploadAsync("tattoo", "f.png", data, "image/png", name: "Sunset", overwrite: true);

        Assert.That(handler.Body, Does.Contain("name=name"));
        Assert.That(handler.Body, Does.Contain("Sunset"));
        Assert.That(handler.Body, Does.Contain("name=overwrite"));
        Assert.That(resp.PublicId, Is.EqualTo("sunset"));
    }
}
