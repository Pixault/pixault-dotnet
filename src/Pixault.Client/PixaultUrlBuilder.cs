namespace Pixault.Client;

/// <summary>
/// Fluent API for generating Pixault image transformation URLs.
/// </summary>
/// <example>
/// <code>
/// // Named transform (watermark locked server-side):
/// var url = images.For("tattoo", "img_01JKABC")
///     .Transform("gallery")
///     .Width(800)
///     .Build();
/// // => "https://img.pixault.io/tattoo/img_01JKABC/t_gallery,w_800.webp"
///
/// // Direct parameters (when allowed by project config):
/// var url = images.For("tattoo", "img_01JKABC")
///     .Width(800)
///     .Quality(85)
///     .Build();
/// // => "https://img.pixault.io/tattoo/img_01JKABC/w_800,q_85.webp"
/// </code>
/// </example>
public sealed class PixaultUrlBuilder
{
    private readonly string _baseUrl;
    private readonly string _project;
    private readonly string _imageId;
    private readonly List<string> _params = [];
    private string _format = "webp";
    private string? _transform;

    public PixaultUrlBuilder(string baseUrl, string project, string imageId)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _project = project;
        _imageId = imageId;
    }

    /// <summary>
    /// Applies a named transform preset. Watermark and other locked parameters
    /// are enforced server-side and cannot be stripped from the URL.
    /// Additional parameters (Width, Quality, etc.) act as overrides for
    /// unlocked preset values.
    /// </summary>
    public PixaultUrlBuilder Transform(string name)
    {
        _transform = name;
        return this;
    }

    public PixaultUrlBuilder Width(int w)
    {
        _params.Add($"w_{w}");
        return this;
    }

    public PixaultUrlBuilder Height(int h)
    {
        _params.Add($"h_{h}");
        return this;
    }

    public PixaultUrlBuilder Fit(FitMode mode)
    {
        _params.Add($"fit_{mode.ToUrlString()}");
        return this;
    }

    public PixaultUrlBuilder Quality(int q)
    {
        _params.Add($"q_{q}");
        return this;
    }

    public PixaultUrlBuilder Blur(int radius)
    {
        _params.Add($"blur_{radius}");
        return this;
    }

    public PixaultUrlBuilder Watermark(string id, WmPosition pos = WmPosition.BottomRight, int opacity = 30)
    {
        _params.Add($"wm_{id}");
        _params.Add($"wm_pos_{pos.ToUrlString()}");
        _params.Add($"wm_opacity_{opacity}");
        return this;
    }

    public PixaultUrlBuilder Format(string fmt)
    {
        _format = fmt.TrimStart('.');
        return this;
    }

    /// <summary>
    /// Builds the final CDN URL for this image transformation.
    /// </summary>
    public string Build()
    {
        var allParams = new List<string>();

        if (_transform is not null)
            allParams.Add($"t_{_transform}");

        allParams.AddRange(_params);

        if (allParams.Count == 0)
            return $"{_baseUrl}/{_project}/{_imageId}/original.{_format}";

        return $"{_baseUrl}/{_project}/{_imageId}/{string.Join(",", allParams)}.{_format}";
    }

    /// <summary>
    /// Generates a single <c>&lt;img&gt;</c> tag with <c>srcset</c> using auto-format negotiation.
    /// The browser picks the best width; the CDN picks the best format via <c>Vary: Accept</c>.
    /// </summary>
    public string ToImgTag(string alt, int[]? widths = null, string sizes = "100vw",
        string loading = "lazy", string? cssClass = null)
    {
        widths ??= [400, 800, 1200];
        var maxWidth = widths.Max();
        var transformParams = BuildTransformParams();

        var srcset = string.Join(", ",
            widths.OrderBy(w => w).Select(w =>
                $"{_baseUrl}/{_project}/{_imageId}/{transformParams}w_{w}.auto {w}w"));
        var src = $"{_baseUrl}/{_project}/{_imageId}/{transformParams}w_{maxWidth}.auto";
        var cls = cssClass is not null ? $" class=\"{Encode(cssClass)}\"" : "";

        return $"<img src=\"{src}\" srcset=\"{srcset}\" sizes=\"{Encode(sizes)}\" alt=\"{Encode(alt)}\" width=\"{maxWidth}\" loading=\"{loading}\" decoding=\"async\"{cls}>";
    }

    /// <summary>
    /// Generates a <c>&lt;picture&gt;</c> element with AVIF and WebP <c>&lt;source&gt;</c> elements
    /// and a JPEG fallback <c>&lt;img&gt;</c>. Provides both format negotiation and responsive sizing.
    /// </summary>
    public string ToPictureTag(string alt, int[]? widths = null, string sizes = "100vw",
        string loading = "lazy", string? cssClass = null)
    {
        widths ??= [400, 800, 1200];
        var maxWidth = widths.Max();
        var transformParams = BuildTransformParams();
        var cls = cssClass is not null ? $" class=\"{Encode(cssClass)}\"" : "";
        var encodedSizes = Encode(sizes);
        var encodedAlt = Encode(alt);

        var avifSrcset = string.Join(", ",
            widths.OrderBy(w => w).Select(w =>
                $"{_baseUrl}/{_project}/{_imageId}/{transformParams}w_{w}.avif {w}w"));
        var webpSrcset = string.Join(", ",
            widths.OrderBy(w => w).Select(w =>
                $"{_baseUrl}/{_project}/{_imageId}/{transformParams}w_{w}.webp {w}w"));
        var fallback = $"{_baseUrl}/{_project}/{_imageId}/{transformParams}w_{maxWidth}.jpg";

        return $"<picture>" +
            $"<source srcset=\"{avifSrcset}\" type=\"image/avif\" sizes=\"{encodedSizes}\">" +
            $"<source srcset=\"{webpSrcset}\" type=\"image/webp\" sizes=\"{encodedSizes}\">" +
            $"<img src=\"{fallback}\" alt=\"{encodedAlt}\" width=\"{maxWidth}\" loading=\"{loading}\" decoding=\"async\"{cls}>" +
            $"</picture>";
    }

    private string BuildTransformParams()
    {
        var parts = new List<string>();
        if (_transform is not null) parts.Add($"t_{_transform}");
        parts.AddRange(_params);
        return parts.Count > 0 ? string.Join(",", parts) + "," : "";
    }

    private static string Encode(string value) =>
        System.Net.WebUtility.HtmlEncode(value);

    public override string ToString() => Build();
}
