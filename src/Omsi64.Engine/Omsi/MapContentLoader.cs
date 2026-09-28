namespace Omsi64.Engine.Omsi;

public static class MapContentLoader
{
    public static OmsiLoadedMap Load(OmsiMap map)
    {
        var contents = map.Tiles.Select(t => TileMapParser.Load(map.MapDirectory, t)).ToArray();
        var omsiRoot = TryFindOmsiRoot(map.MapDirectory);
        var missing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (omsiRoot is not null)
        {
            foreach (var asset in contents.SelectMany(t =>
                         t.Objects.Select(o => o.AssetPath).Concat(t.Splines.Select(s => s.AssetPath))))
            {
                var normalized = asset.Replace('\\', Path.DirectorySeparatorChar)
                                      .Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(omsiRoot, normalized);

                if (!File.Exists(fullPath))
                    missing.Add(asset);
            }
        }

        return new OmsiLoadedMap
        {
            Map = map,
            TileContents = contents,
            MissingAssets = missing.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray()
        };
    }

    private static string? TryFindOmsiRoot(string mapDirectory)
    {
        var mapDir = new DirectoryInfo(mapDirectory);
        var mapsDir = mapDir.Parent;

        if (mapsDir is not null &&
            mapsDir.Name.Equals("maps", StringComparison.OrdinalIgnoreCase) &&
            mapsDir.Parent is not null)
            return mapsDir.Parent.FullName;

        return null;
    }
}