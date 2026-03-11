using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Pixault.Client;

/// <summary>
/// HTTP client for Pixault admin/management API operations.
/// Used by the dashboard components to list, inspect, and manage images and transforms.
/// </summary>
public sealed class PixaultAdminClient
{
    private readonly HttpClient _http;
    private readonly PixaultOptions _options;

    public PixaultAdminClient(HttpClient http, IOptions<PixaultOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    private string Project => _options.DefaultProject ?? throw new InvalidOperationException("DefaultProject must be set");

    // ── Images ───────────────────────────────────────────────────

    public async Task<ImageListResponse> ListImagesAsync(
        int limit = 50, string? cursor = null, string? project = null,
        string? search = null, string? category = null, string? keyword = null,
        string? author = null, bool? isVideo = null, string? folder = null,
        CancellationToken ct = default)
    {
        var p = project ?? Project;
        var url = $"/api/{p}/images?limit={limit}";
        if (cursor is not null)
            url += $"&cursor={Uri.EscapeDataString(cursor)}";
        if (search is not null)
            url += $"&search={Uri.EscapeDataString(search)}";
        if (category is not null)
            url += $"&category={Uri.EscapeDataString(category)}";
        if (keyword is not null)
            url += $"&keyword={Uri.EscapeDataString(keyword)}";
        if (author is not null)
            url += $"&author={Uri.EscapeDataString(author)}";
        if (isVideo.HasValue)
            url += $"&isVideo={isVideo.Value.ToString().ToLowerInvariant()}";
        if (folder is not null)
            url += $"&folder={Uri.EscapeDataString(folder)}";

        return await _http.GetFromJsonAsync<ImageListResponse>(url, ct)
            ?? new ImageListResponse();
    }

    public async Task<ImageMetadataDto?> GetMetadataAsync(string imageId, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        try
        {
            return await _http.GetFromJsonAsync<ImageMetadataDto>($"/api/{p}/{imageId}/metadata", ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<ImageMetadataDto?> UpdateMetadataAsync(string imageId, MetadataUpdate update, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.PatchAsJsonAsync($"/api/{p}/{imageId}/metadata", update, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ImageMetadataDto>(ct);
    }

    public async Task<List<string>> ListFoldersAsync(string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        return await _http.GetFromJsonAsync<List<string>>($"/api/{p}/folders", ct) ?? [];
    }

    public async Task CreateFolderAsync(string folderPath, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.PostAsJsonAsync($"/api/{p}/folders", new { path = folderPath }, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteFolderAsync(string folderPath, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.DeleteAsync($"/api/{p}/folders/{Uri.EscapeDataString(folderPath)}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteImageAsync(string imageId, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.DeleteAsync($"/api/{p}/{imageId}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ImageMetadataDto?> StripExifAsync(string imageId, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.PostAsync($"/api/{p}/{imageId}/strip-exif", null, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ImageMetadataDto>(ct);
    }

    // ── Named Transforms ─────────────────────────────────────────

    public async Task<List<NamedTransformDto>> ListTransformsAsync(string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        return await _http.GetFromJsonAsync<List<NamedTransformDto>>($"/api/{p}/transforms", ct) ?? [];
    }

    public async Task<NamedTransformDto?> GetTransformAsync(string name, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        try
        {
            return await _http.GetFromJsonAsync<NamedTransformDto>($"/api/{p}/transforms/{name}", ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<NamedTransformDto?> SaveTransformAsync(string name, NamedTransformSave save, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.PutAsJsonAsync($"/api/{p}/transforms/{name}", save, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NamedTransformDto>(ct);
    }

    public async Task DeleteTransformAsync(string name, string? project = null, CancellationToken ct = default)
    {
        var p = project ?? Project;
        var response = await _http.DeleteAsync($"/api/{p}/transforms/{name}", ct);
        response.EnsureSuccessStatusCode();
    }

    // ── Plugins ───────────────────────────────────────────────────

    public async Task<List<PluginDto>> GetAllPluginsAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<List<PluginDto>>("/api/plugins", ct) ?? [];
    }

    public async Task<List<ProjectPluginDto>> GetProjectPluginsAsync(string? projectId = null, CancellationToken ct = default)
    {
        var t = projectId ?? Project;
        return await _http.GetFromJsonAsync<List<ProjectPluginDto>>($"/api/{t}/plugins", ct) ?? [];
    }

    public async Task ActivatePluginAsync(string projectId, string pluginName, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"/api/{projectId}/plugins/{pluginName}/activate", null, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeactivatePluginAsync(string projectId, string pluginName, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"/api/{projectId}/plugins/{pluginName}/deactivate", null, ct);
        response.EnsureSuccessStatusCode();
    }
}

// ── DTOs ─────────────────────────────────────────────────────────

public sealed class ImageListResponse
{
    public List<ImageMetadataDto> Images { get; set; } = [];
    public string? NextCursor { get; set; }
    public int TotalCount { get; set; }
}

public sealed class ImageMetadataDto
{
    public string ImageId { get; set; } = "";
    public string ProjectId { get; set; } = "";
    public string OriginalFileName { get; set; } = "";
    public DateTimeOffset UploadedAt { get; set; }
    public string ContentType { get; set; } = "";
    public long SizeBytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Caption { get; set; }
    public string? Category { get; set; }
    public string? Folder { get; set; }
    public List<string>? Keywords { get; set; }
    public string? Author { get; set; }
    public string? CopyrightHolder { get; set; }
    public int? CopyrightYear { get; set; }
    public string? License { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public DateTimeOffset? DatePublished { get; set; }
    public DateTimeOffset? DateModified { get; set; }
    public bool? RepresentativeOfPage { get; set; }
    public Dictionary<string, string>? ExifData { get; set; }
    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }
    public string? LocationName { get; set; }
    public Dictionary<string, string>? Tags { get; set; }

    // Video fields
    public bool IsVideo { get; set; }
    public double? Duration { get; set; }
    public bool? HasAudio { get; set; }
    public string? ThumbnailId { get; set; }

    public string FormattedSize => SizeBytes switch
    {
        < 1024 => $"{SizeBytes} B",
        < 1024 * 1024 => $"{SizeBytes / 1024.0:F1} KB",
        _ => $"{SizeBytes / (1024.0 * 1024.0):F1} MB"
    };

    public bool IsSvg => ContentType == "image/svg+xml";

    public string FormattedDuration => Duration switch
    {
        null => "",
        < 60 => $"{Duration:F0}s",
        < 3600 => $"{(int)(Duration / 60)}:{(int)(Duration % 60):D2}",
        _ => $"{(int)(Duration / 3600)}:{(int)(Duration % 3600 / 60):D2}:{(int)(Duration % 60):D2}"
    };
}

public sealed class MetadataUpdate
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Caption { get; set; }
    public string? Category { get; set; }
    public string? Folder { get; set; }
    public List<string>? Keywords { get; set; }
    public string? Author { get; set; }
    public string? CopyrightHolder { get; set; }
    public int? CopyrightYear { get; set; }
    public string? License { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public DateTimeOffset? DatePublished { get; set; }
    public bool? RepresentativeOfPage { get; set; }
    public Dictionary<string, string>? ExifData { get; set; }
    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }
    public string? LocationName { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
}

public sealed class NamedTransformDto
{
    public string Name { get; set; } = "";
    public string ProjectId { get; set; } = "";
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? FitMode { get; set; }
    public int? Quality { get; set; }
    public int? Blur { get; set; }
    public string? WatermarkId { get; set; }
    public string? WatermarkPosition { get; set; }
    public int? WatermarkOpacity { get; set; }
    public HashSet<string> LockedParameters { get; set; } = [];
}

public sealed class NamedTransformSave
{
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? FitMode { get; set; }
    public int? Quality { get; set; }
    public int? Blur { get; set; }
    public string? WatermarkId { get; set; }
    public string? WatermarkPosition { get; set; }
    public int? WatermarkOpacity { get; set; }
    public HashSet<string>? LockedParameters { get; set; }
}

public sealed class PluginDto
{
    public string Name { get; set; } = "";
    public string Vendor { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Stage { get; set; } = "";
    public int PriceCentsPerInvocation { get; set; }
    public string UrlPrefix { get; set; } = "";
}

public sealed class ProjectPluginDto
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Vendor { get; set; } = "";
    public string Stage { get; set; } = "";
    public int PriceCentsPerInvocation { get; set; }
    public string UrlPrefix { get; set; } = "";
    public bool IsActivated { get; set; }
}
