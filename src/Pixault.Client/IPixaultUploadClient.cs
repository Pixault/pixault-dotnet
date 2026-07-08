namespace Pixault.Client;

/// <summary>
/// Upload operations against a Pixault project.
///
/// The default implementation (<see cref="PixaultUploadClient"/>) calls the Pixault REST
/// API over HTTP. Hosts that run in the same process as the API (e.g. the Pixault
/// dashboard) may register an in-process implementation to skip the network hop.
/// </summary>
public interface IPixaultUploadClient
{
    /// <summary>Uploads an image to Pixault and returns the new image ID.</summary>
    Task<UploadResponse> UploadAsync(
        string project, string fileName, Stream data, string contentType,
        string? folder = null, CancellationToken ct = default);

    /// <summary>Deletes an image from Pixault.</summary>
    Task DeleteAsync(string project, string imageId, CancellationToken ct = default);
}
