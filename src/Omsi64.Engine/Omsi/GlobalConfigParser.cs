using System.Globalization;

namespace Omsi64.Engine.Omsi;

public static class GlobalConfigParser
{
    public static OmsiMap Load(string globalCfgPath)
    {
        globalCfgPath = Path.GetFullPath(globalCfgPath);
        var lines = OmsiText.ReadMeaningfulLines(globalCfgPath);

        var tiles = new List<OmsiMapTile>();
        string? friendlyName = null;
        string? internalName = null;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (line.Equals("[friendlyname]", StringComparison.OrdinalIgnoreCase))
            {
                friendlyName = Next(lines, ref i);
                continue;
            }

            if (line.Equals("[name]", StringComparison.OrdinalIgnoreCase))
            {
                internalName = Next(lines, ref i);
                continue;
            }

            if (!line.Equals("[map]", StringComparison.OrdinalIgnoreCase))
                continue;

            var xRaw = Next(lines, ref i);
            var yRaw = Next(lines, ref i);
            var fileName = Next(lines, ref i);

            if (!int.TryParse(xRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var x) ||
                !int.TryParse(yRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
                continue;

            tiles.Add(new OmsiMapTile(x, y, fileName));
        }

        var dir = Path.GetDirectoryName(globalCfgPath)
                  ?? throw new InvalidOperationException("Nie udało się ustalić folderu mapy.");

        return new OmsiMap
        {
            Name = friendlyName ?? internalName ?? Path.GetFileName(dir),
            MapDirectory = dir,
            GlobalConfigPath = globalCfgPath,
            Tiles = tiles
        };
    }

    private static string Next(string[] lines, ref int index) =>
        ++index < lines.Length ? lines[index] : string.Empty;
}