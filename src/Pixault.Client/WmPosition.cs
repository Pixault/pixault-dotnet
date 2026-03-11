namespace Pixault.Client;

public enum WmPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center,
    Tile
}

internal static class WmPositionExtensions
{
    public static string ToUrlString(this WmPosition pos) => pos switch
    {
        WmPosition.TopLeft => "tl",
        WmPosition.TopRight => "tr",
        WmPosition.BottomLeft => "bl",
        WmPosition.BottomRight => "br",
        WmPosition.Center => "c",
        WmPosition.Tile => "tile",
        _ => "br"
    };
}
