using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Pixault.Client;

/// <summary>
/// HTTP client for Pixault upload and management API operations.
/// </summary>
public sealed class PixaultUploadClient
{
    private readonly HttpClient _http;
    private readonly PixaultOptions _options;

    public PixaultUploadClient(HttpClient http, IOptions<PixaultOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    /// <summary>
    /// Uploads an image to Pixault.
    /// </summary>
    /// <param name="project">Project identifier</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="data">Image data stream</param>
    /// <param name="contentType">MIME type of the image</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The upload response containing the new image ID</returns>
    public async Task<UploadResponse> UploadAsync(
        string project, string fileName, Stream data, string contentType,
        string? folder = null, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(data);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);

        if (!string.IsNullOrWhiteSpace(folder))
            content.Add(new StringContent(folder), "folder");

        var response = await _http.PostAsync($"/api/{project}/upload", content, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UploadResponse>(ct)
            ?? throw new InvalidOperationException("Invalid upload response");
    }

    /// <summary>
    /// Deletes an image from Pixault.
    /// </summary>
    public async Task DeleteAsync(string project, string imageId, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/api/{project}/{imageId}", ct);
        response.EnsureSuccessStatusCode();
    }
}

public sealed record UploadResponse(string ImageId, string Url);
