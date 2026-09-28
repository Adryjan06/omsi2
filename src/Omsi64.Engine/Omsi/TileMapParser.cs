using System.Globalization;

namespace Omsi64.Engine.Omsi;

public static class TileMapParser
{
    public static OmsiTileContent Load(string mapDirectory, OmsiMapTile tile)
    {
        var filePath = Path.Combine(mapDirectory, tile.FileName);
        var objects = new List<OmsiPlacedObject>();
        var splines = new List<OmsiPlacedSpline>();
        var warnings = new List<string>();

        if (!File.Exists(filePath))
        {
            warnings.Add($"Brak pliku kafelka: {filePath}");
            return new OmsiTileContent
            {
                Tile = tile, FilePath = filePath,
                Objects = objects, Splines = splines, Warnings = warnings
            };
        }

        var lines = OmsiText.ReadMeaningfulLines(filePath);

        for (var i = 0; i < lines.Length; i++)
        {
            var tag = lines[i];

            if (tag.Equals("[object]", StringComparison.OrdinalIgnoreCase))
            {
                var values = ReadBlock(lines, i + 1);
                if (TryParseObject(values, out var obj, out var error))
                    objects.Add(obj!);
                else
                    warnings.Add($"{tile.FileName}: [object] przy linii logicznej {i + 1}: {error}");
                continue;
            }

            if (tag.Equals("[spline]", StringComparison.OrdinalIgnoreCase) ||
                tag.Equals("[spline_h]", StringComparison.OrdinalIgnoreCase))
            {
                var values = ReadBlock(lines, i + 1);
                var isHeight = tag.Equals("[spline_h]", StringComparison.OrdinalIgnoreCase);
                if (TryParseSpline(values, isHeight, out var spline, out var error))
                    splines.Add(spline!);
                else
                    warnings.Add($"{tile.FileName}: {tag} przy linii logicznej {i + 1}: {error}");
            }
        }

        return new OmsiTileContent
        {
            Tile = tile, FilePath = filePath,
            Objects = objects, Splines = splines, Warnings = warnings
        };
    }

    private static List<string> ReadBlock(string[] lines, int start)
    {
        var values = new List<string>();
        for (var i = start; i < lines.Length; i++)
        {
            if (lines[i].StartsWith('[', StringComparison.Ordinal))
                break;
            values.Add(lines[i]);
        }
        return values;
    }

    private static bool TryParseObject(IReadOnlyList<string> v, out OmsiPlacedObject? obj, out string error)
    {
        obj = null;
        error = string.Empty;

        if (v.Count < 9)
        {
            error = $"za mało wartości ({v.Count}, oczekiwano co najmniej 9)";
            return false;
        }

        if (!TryLong(v[2], out var id) ||
            !TryDouble(v[3], out var x) || !TryDouble(v[4], out var y) || !TryDouble(v[5], out var z) ||
            !TryDouble(v[6], out var rot) || !TryDouble(v[7], out var pitch) || !TryDouble(v[8], out var bank))
        {
            error = "nie udało się odczytać ID/pozycji/rotacji";
            return false;
        }

        obj = new OmsiPlacedObject(v[1], id, x, y, z, rot, pitch, bank);
        return true;
    }

    private static bool TryParseSpline(IReadOnlyList<string> v, bool isHeightSpline, out OmsiPlacedSpline? spline, out string error)
    {
        spline = null;
        error = string.Empty;

        if (v.Count < 13)
        {
            error = $"za mało wartości ({v.Count}, oczekiwano co najmniej 13)";
            return false;
        }

        if (!TryLong(v[2], out var id) || !TryLong(v[3], out var prev) || !TryLong(v[4], out var next) ||
            !TryDouble(v[5], out var x) || !TryDouble(v[6], out var z) || !TryDouble(v[7], out var y) ||
            !TryDouble(v[8], out var rot) || !TryDouble(v[9], out var length) || !TryDouble(v[10], out var radius) ||
            !TryDouble(v[11], out var gradientStart) || !TryDouble(v[12], out var gradientEnd))
        {
            error = "nie udało się odczytać ID/geometrii spline'u";
            return false;
        }

        var mirrored = v.Any(x => x.Equals("mirror", StringComparison.OrdinalIgnoreCase));

        spline = new OmsiPlacedSpline(
            v[1], id, prev, next, x, y, z, rot, length, radius,
            gradientStart, gradientEnd, isHeightSpline, mirrored);

        return true;
    }

    private static bool TryDouble(string raw, out double value) =>
        double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

    private static bool TryLong(string raw, out long value) =>
        long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
}