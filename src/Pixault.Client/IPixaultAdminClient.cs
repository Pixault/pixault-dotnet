namespace Pixault.Client;

/// <summary>
/// Admin/management operations against a Pixault project — listing, inspecting, and
/// managing images, folders, transforms, watermarks, and plugins.
///
/// The default implementation (<see cref="PixaultAdminClient"/>) calls the Pixault REST
/// API over HTTP. Hosts that run in the same process as the API (e.g. the Pixault
/// dashboard) may register an in-process implementation to skip the network hop.
/// </summary>
public interface IPixaultAdminClient
{
    // ── Images ───────────────────────────────────────────────────
    Task<ImageListResponse> ListImagesAsync(
        int limit = 50, string? cursor = null, string? project = null,
        string? search = null, string? category = null, string? keyword = null,
        string? author = null, bool? isVideo = null, string? folder = null,
        CancellationToken ct = default);

    Task<ImageMetadataDto?> GetMetadataAsync(string imageId, string? project = null, CancellationToken ct = default);

    Task<ImageMetadataDto?> UpdateMetadataAsync(string imageId, MetadataUpdate update, string? project = null, CancellationToken ct = default);

    Task<List<string>> ListFoldersAsync(string? project = null, CancellationToken ct = default);

    Task CreateFolderAsync(string folderPath, string? project = null, CancellationToken ct = default);

    Task DeleteFolderAsync(string folderPath, string? project = null, CancellationToken ct = default);

    Task DeleteImageAsync(string imageId, string? project = null, CancellationToken ct = default);

    Task<ImageMetadataDto?> StripExifAsync(string imageId, string? project = null, CancellationToken ct = default);

    // ── EPS ──────────────────────────────────────────────────────
    Task<List<DerivedAssetDto>> GetDerivedAssetsAsync(string imageId, string? project = null, CancellationToken ct = default);

    Task<EpsProcessingStatusDto?> GetEpsProcessingStatusAsync(string imageId, string? project = null, CancellationToken ct = default);

    Task SplitEpsDesignsAsync(string imageId, string? project = null, CancellationToken ct = default);

    Task ExtractEpsSvgAsync(string imageId, string? project = null, CancellationToken ct = default);

    // ── Named Transforms ─────────────────────────────────────────
    Task<List<NamedTransformDto>> ListTransformsAsync(string? project = null, CancellationToken ct = default);

    Task<NamedTransformDto?> GetTransformAsync(string name, string? project = null, CancellationToken ct = default);

    Task<NamedTransformDto?> SaveTransformAsync(string name, NamedTransformSave save, string? project = null, CancellationToken ct = default);

    Task DeleteTransformAsync(string name, string? project = null, CancellationToken ct = default);

    // ── Watermarks ───────────────────────────────────────────────
    Task<List<WatermarkDto>> ListWatermarksAsync(string? project = null, CancellationToken ct = default);

    Task<WatermarkDto?> UploadWatermarkAsync(
        string watermarkId, Stream imageStream, string contentType = "image/png",
        string? project = null, CancellationToken ct = default);

    Task DeleteWatermarkAsync(string watermarkId, string? project = null, CancellationToken ct = default);

    // ── Plugins ──────────────────────────────────────────────────
    Task<List<PluginDto>> GetAllPluginsAsync(CancellationToken ct = default);

    Task<List<ProjectPluginDto>> GetProjectPluginsAsync(string? projectId = null, CancellationToken ct = default);

    Task ActivatePluginAsync(string projectId, string pluginName, CancellationToken ct = default);

    Task DeactivatePluginAsync(string projectId, string pluginName, CancellationToken ct = default);
}
