using Microsoft.Extensions.Options;

namespace Pixault.Client;

/// <summary>
/// Main service for generating Pixault image URLs.
/// Registered as a singleton via DI.
/// </summary>
public sealed class PixaultImageService
{
    private readonly PixaultOptions _options;
    private readonly string _publicUrl;

    public PixaultImageService(IOptions<PixaultOptions> options)
    {
        _options = options.Value;
        _publicUrl = _options.CdnUrl ?? _options.BaseUrl;
    }

    /// <summary>
    /// Creates a URL builder for the specified project and image.
    /// </summary>
    /// <param name="project">The project identifier (e.g., "barber", "tattoo")</param>
    /// <param name="imageId">The unique image identifier</param>
    public PixaultUrlBuilder For(string project, string imageId)
        => new(_publicUrl, project, imageId);

    /// <summary>
    /// Creates a URL builder using the default project.
    /// </summary>
    /// <param name="imageId">The unique image identifier</param>
    public PixaultUrlBuilder For(string imageId)
    {
        if (string.IsNullOrEmpty(_options.DefaultProject))
            throw new InvalidOperationException("DefaultProject must be configured when using For(imageId) without a project parameter.");

        return new PixaultUrlBuilder(_publicUrl, _options.DefaultProject, imageId);
    }

    /// <summary>
    /// Generates a video streaming URL for the specified project and video.
    /// </summary>
    public string VideoUrl(string project, string videoId, string contentType)
    {
        var ext = contentType switch
        {
            "video/mp4" => "mp4",
            "video/webm" => "webm",
            "video/quicktime" => "mov",
            _ => "mp4"
        };
        return $"{_publicUrl}/{project}/{videoId}/video.{ext}";
    }
}
