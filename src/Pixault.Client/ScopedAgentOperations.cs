namespace Pixault.Client;

/// <summary>
/// Decorator over <see cref="IAgentOperations"/> that enforces
/// <see cref="AgentPermissions"/> before allowing write/destructive operations.
/// Read operations and URL generation always pass through.
/// </summary>
public sealed class ScopedAgentOperations : IAgentOperations
{
    private readonly IAgentOperations _inner;
    private readonly AgentPermissions _permissions;

    public ScopedAgentOperations(IAgentOperations inner, AgentPermissions permissions)
    {
        _inner = inner;
        _permissions = permissions;
    }

    // ── Read (always allowed) ─────────────────────────────────────

    public Task<ImageListResponse> ListImagesAsync(int limit, string? cursor, CancellationToken ct)
        => _inner.ListImagesAsync(limit, cursor, ct);

    public Task<ImageMetadataDto?> GetMetadataAsync(string imageId, CancellationToken ct)
        => _inner.GetMetadataAsync(imageId, ct);

    public Task<List<NamedTransformDto>> ListTransformsAsync(CancellationToken ct)
        => _inner.ListTransformsAsync(ct);

    public Task<List<PluginDto>> GetAllPluginsAsync(CancellationToken ct)
        => _inner.GetAllPluginsAsync(ct);

    public Task<List<ProjectPluginDto>> GetProjectPluginsAsync(CancellationToken ct)
        => _inner.GetProjectPluginsAsync(ct);

    // ── Write (requires AllowWrite) ───────────────────────────────

    public Task<ImageMetadataDto?> UpdateMetadataAsync(string imageId, MetadataUpdate update, CancellationToken ct)
    {
        RequireWrite();
        return _inner.UpdateMetadataAsync(imageId, update, ct);
    }

    public Task<NamedTransformDto?> SaveTransformAsync(string name, NamedTransformSave save, CancellationToken ct)
    {
        RequireWrite();
        return _inner.SaveTransformAsync(name, save, ct);
    }

    // ── Plugin management (requires AllowPluginManagement) ────────

    public Task ActivatePluginAsync(string project, string pluginName, CancellationToken ct)
    {
        RequirePluginManagement();
        return _inner.ActivatePluginAsync(project, pluginName, ct);
    }

    public Task DeactivatePluginAsync(string project, string pluginName, CancellationToken ct)
    {
        RequirePluginManagement();
        return _inner.DeactivatePluginAsync(project, pluginName, ct);
    }

    // ── Destructive (requires AllowDelete) ────────────────────────

    public Task DeleteImageAsync(string imageId, CancellationToken ct)
    {
        RequireDelete();
        return _inner.DeleteImageAsync(imageId, ct);
    }

    public Task DeleteTransformAsync(string name, CancellationToken ct)
    {
        RequireDelete();
        return _inner.DeleteTransformAsync(name, ct);
    }

    // ── URL generation (always allowed) ───────────────────────────

    public string BuildImageUrl(string project, string imageId, Action<PixaultUrlBuilder>? configure)
        => _inner.BuildImageUrl(project, imageId, configure);

    // ── Permission checks ─────────────────────────────────────────

    private void RequireWrite()
    {
        if (!_permissions.AllowWrite)
            throw new UnauthorizedAccessException("Agent does not have write permissions.");
    }

    private void RequireDelete()
    {
        if (!_permissions.AllowDelete)
            throw new UnauthorizedAccessException("Agent does not have delete permissions.");
    }

    private void RequirePluginManagement()
    {
        if (!_permissions.AllowPluginManagement)
            throw new UnauthorizedAccessException("Agent does not have plugin management permissions.");
    }
}
