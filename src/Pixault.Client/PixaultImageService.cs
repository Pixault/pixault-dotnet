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
    /// <param name="publicId">The unique image identifier</param>
    public PixaultUrlBuilder For(string project, string publicId)
        => new(_publicUrl, project, publicId);

    /// <summary>
    /// Creates a URL builder using the default project.
    /// </summary>
    /// <param name="publicId">The unique image identifier</param>
    public PixaultUrlBuilder For(string publicId)
    {
        if (string.IsNullOrEmpty(_options.DefaultProject))
            throw new InvalidOperationException("DefaultProject must be configured when using For(publicId) without a project parameter.");

        return new PixaultUrlBuilder(_publicUrl, _options.DefaultProject, publicId);
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
