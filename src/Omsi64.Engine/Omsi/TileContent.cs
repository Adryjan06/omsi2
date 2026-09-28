namespace Omsi64.Engine.Omsi;

public sealed record OmsiPlacedObject(
    string AssetPath, long Id, double X, double Y, double Z,
    double Rotation, double Pitch, double Bank);

public sealed record OmsiPlacedSpline(
    string AssetPath, long Id, long PreviousId, long NextId,
    double X, double Y, double Z, double Rotation,
    double Length, double Radius, double GradientStart, double GradientEnd,
    bool IsHeightSpline, bool Mirrored);

public sealed class OmsiTileContent
{
    public required OmsiMapTile Tile { get; init; }
    public required string FilePath { get; init; }
    public required IReadOnlyList<OmsiPlacedObject> Objects { get; init; }
    public required IReadOnlyList<OmsiPlacedSpline> Splines { get; init; }
    public required IReadOnlyList<string> Warnings { get; init; }
}

public sealed class OmsiLoadedMap
{
    public required OmsiMap Map { get; init; }
    public required IReadOnlyList<OmsiTileContent> TileContents { get; init; }
    public required IReadOnlyList<string> MissingAssets { get; init; }
}