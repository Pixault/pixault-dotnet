namespace Pixault.Client;

/// <summary>
/// Abstraction over the Pixault operations available to the AI agent.
/// Decouples the agent from concrete admin clients and enables permission scoping
/// via <see cref="ScopedAgentOperations"/>.
/// </summary>
public interface IAgentOperations
{
    // ── Read ──────────────────────────────────────────────────────

    Task<ImageListResponse> ListImagesAsync(int limit = 20, string? cursor = null, CancellationToken ct = default);
    Task<ImageMetadataDto?> GetMetadataAsync(string imageId, CancellationToken ct = default);
    Task<List<NamedTransformDto>> ListTransformsAsync(CancellationToken ct = default);
    Task<List<PluginDto>> GetAllPluginsAsync(CancellationToken ct = default);
    Task<List<ProjectPluginDto>> GetProjectPluginsAsync(CancellationToken ct = default);

    // ── Write ─────────────────────────────────────────────────────

    Task<ImageMetadataDto?> UpdateMetadataAsync(string imageId, MetadataUpdate update, CancellationToken ct = default);
    Task<NamedTransformDto?> SaveTransformAsync(string name, NamedTransformSave save, CancellationToken ct = default);
    Task ActivatePluginAsync(string project, string pluginName, CancellationToken ct = default);
    Task DeactivatePluginAsync(string project, string pluginName, CancellationToken ct = default);

    // ── Destructive ───────────────────────────────────────────────

    Task DeleteImageAsync(string imageId, CancellationToken ct = default);
    Task DeleteTransformAsync(string name, CancellationToken ct = default);

    // ── URL generation ────────────────────────────────────────────

    string BuildImageUrl(string project, string imageId, Action<PixaultUrlBuilder>? configure = null);
}
