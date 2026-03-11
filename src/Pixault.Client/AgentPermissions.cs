namespace Pixault.Client;

/// <summary>
/// Controls which categories of operations the AI agent is allowed to perform.
/// Used by <see cref="ScopedAgentOperations"/> to enforce permission boundaries.
/// </summary>
public record AgentPermissions
{
    /// <summary>Allow destructive operations (delete image, delete transform).</summary>
    public bool AllowDelete { get; init; } = false;

    /// <summary>Allow write operations (update metadata, save transform).</summary>
    public bool AllowWrite { get; init; } = true;

    /// <summary>Allow plugin management (activate/deactivate plugins).</summary>
    public bool AllowPluginManagement { get; init; } = false;
}
