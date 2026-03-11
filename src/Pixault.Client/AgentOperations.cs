namespace Pixault.Client;

/// <summary>
/// Default <see cref="IAgentOperations"/> implementation that delegates directly
/// to <see cref="PixaultAdminClient"/> and <see cref="PixaultImageService"/>.
/// Provides full, unrestricted access to all operations.
/// </summary>
public sealed class AgentOperations : IAgentOperations
{
    private readonly PixaultAdminClient _admin;
    private readonly PixaultImageService _images;

    public AgentOperations(PixaultAdminClient admin, PixaultImageService images)
    {
        _admin = admin;
        _images = images;
    }

    // ── Read ──────────────────────────────────────────────────────

    public Task<ImageListResponse> ListImagesAsync(int limit, string? cursor, CancellationToken ct)
        => _admin.ListImagesAsync(limit, cursor, ct: ct);

    public Task<ImageMetadataDto?> GetMetadataAsync(string imageId, CancellationToken ct)
        => _admin.GetMetadataAsync(imageId, ct: ct);

    public Task<List<NamedTransformDto>> ListTransformsAsync(CancellationToken ct)
        => _admin.ListTransformsAsync(ct: ct);

    public Task<List<PluginDto>> GetAllPluginsAsync(CancellationToken ct)
        => _admin.GetAllPluginsAsync(ct);

    public Task<List<ProjectPluginDto>> GetProjectPluginsAsync(CancellationToken ct)
        => _admin.GetProjectPluginsAsync(ct: ct);

    // ── Write ─────────────────────────────────────────────────────

    public Task<ImageMetadataDto?> UpdateMetadataAsync(string imageId, MetadataUpdate update, CancellationToken ct)
        => _admin.UpdateMetadataAsync(imageId, update, ct: ct);

    public Task<NamedTransformDto?> SaveTransformAsync(string name, NamedTransformSave save, CancellationToken ct)
        => _admin.SaveTransformAsync(name, save, ct: ct);

    public Task ActivatePluginAsync(string project, string pluginName, CancellationToken ct)
        => _admin.ActivatePluginAsync(project, pluginName, ct);

    public Task DeactivatePluginAsync(string project, string pluginName, CancellationToken ct)
        => _admin.DeactivatePluginAsync(project, pluginName, ct);

    // ── Destructive ───────────────────────────────────────────────

    public Task DeleteImageAsync(string imageId, CancellationToken ct)
        => _admin.DeleteImageAsync(imageId, ct: ct);

    public Task DeleteTransformAsync(string name, CancellationToken ct)
        => _admin.DeleteTransformAsync(name, ct: ct);

    // ── URL generation ────────────────────────────────────────────

    public string BuildImageUrl(string project, string imageId, Action<PixaultUrlBuilder>? configure)
    {
        var builder = _images.For(project, imageId);
        configure?.Invoke(builder);
        return builder.Build();
    }
}
