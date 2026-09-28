namespace Omsi64.Engine.Omsi;

public sealed record OmsiMapTile(int X, int Y, string FileName)
{
    public const float SizeMeters = 300f;
}

public sealed class OmsiMap
{
    public required string Name { get; init; }
    public required string MapDirectory { get; init; }
    public required string GlobalConfigPath { get; init; }
    public required IReadOnlyList<OmsiMapTile> Tiles { get; init; }
}