namespace Pixault.Client;

/// <summary>
/// Configuration options for the Pixault client.
/// </summary>
public sealed class PixaultOptions
{
    /// <summary>
    /// Base URL for server-side API calls (e.g., "http://localhost:8080").
    /// Used by PixaultUploadClient and PixaultAdminClient for HTTP requests.
    /// </summary>
    public string BaseUrl { get; set; } = "https://img.pixault.io";

    /// <summary>
    /// Public CDN URL for browser-facing image URLs (e.g., "https://img.pixault.io").
    /// Used by PixaultImageService to generate &lt;img src="..."&gt; URLs.
    /// Falls back to BaseUrl if not set.
    /// </summary>
    public string? CdnUrl { get; set; }

    /// <summary>
    /// Default project ID to use when not specified per-request.
    /// </summary>
    public string? DefaultProject { get; set; }

    /// <summary>
    /// API key for upload and management operations.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// HMAC secret for generating signed URLs (for protected originals).
    /// </summary>
    public string? HmacSecret { get; set; }
}
