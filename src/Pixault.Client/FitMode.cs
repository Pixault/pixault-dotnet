namespace Pixault.Client;

public enum FitMode
{
    Cover,
    Contain,
    Fill,
    Pad
}

internal static class FitModeUrlExtensions
{
    public static string ToUrlString(this FitMode mode) => mode switch
    {
        FitMode.Cover => "cover",
        FitMode.Contain => "contain",
        FitMode.Fill => "fill",
        FitMode.Pad => "pad",
        _ => "cover"
    };
}
