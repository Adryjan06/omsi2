using Omsi64.Engine.Omsi;

namespace Omsi64.Engine.Rendering;

internal static class DebugGeometryBuilder
{
    public static LineMesh BuildTileMesh(IReadOnlyList<OmsiMapTile> tiles)
    {
        var v = new List<float>();

        foreach (var tile in tiles)
        {
            var x0 = tile.X * OmsiMapTile.SizeMeters;
            var x1 = x0 + OmsiMapTile.SizeMeters;
            var z0 = -tile.Y * OmsiMapTile.SizeMeters;
            var z1 = z0 - OmsiMapTile.SizeMeters;

            AddLine(v, x0, 0, z0, x1, 0, z0);
            AddLine(v, x1, 0, z0, x1, 0, z1);
            AddLine(v, x1, 0, z1, x0, 0, z1);
            AddLine(v, x0, 0, z1, x0, 0, z0);
        }

        return new LineMesh(v);
    }

    public static LineMesh BuildObjectMesh(IReadOnlyList<OmsiTileContent> tiles)
    {
        var v = new List<float>();

        foreach (var content in tiles)
        {
            var ox = content.Tile.X * OmsiMapTile.SizeMeters;
            var oy = content.Tile.Y * OmsiMapTile.SizeMeters;

            foreach (var obj in content.Objects)
            {
                var x = (float)(ox + obj.X);
                var y = (float)obj.Z + 0.15f;
                var z = (float)-(oy + obj.Y);

                AddLine(v, x - 2, y, z, x + 2, y, z);
                AddLine(v, x, y, z - 2, x, y, z + 2);
                AddLine(v, x, y, z, x, y + 3, z);
            }
        }

        return new LineMesh(v);
    }

    public static LineMesh BuildSplineMesh(IReadOnlyList<OmsiTileContent> tiles)
    {
        var v = new List<float>();

        foreach (var content in tiles)
        {
            var ox = content.Tile.X * OmsiMapTile.SizeMeters;
            var oy = content.Tile.Y * OmsiMapTile.SizeMeters;

            foreach (var spline in content.Splines)
            {
                var segments = Math.Clamp((int)Math.Ceiling(Math.Abs(spline.Length) / 5.0), 1, 128);
                var prev = Sample(spline, 0, ox, oy);

                for (var i = 1; i <= segments; i++)
                {
                    var d = spline.Length * i / segments;
                    var current = Sample(spline, d, ox, oy);

                    AddLine(v, prev.x, prev.y + 0.25f, prev.z,
                               current.x, current.y + 0.25f, current.z);

                    prev = current;
                }
            }
        }

        return new LineMesh(v);
    }

    private static (float x, float y, float z) Sample(OmsiPlacedSpline s, double distance, double ox, double oy)
    {
        var a = s.Rotation * Math.PI / 180.0;
        var radius = s.Mirrored ? -s.Radius : s.Radius;

        double lx;
        double ly;

        if (Math.Abs(radius) < 0.000001)
        {
            lx = s.X + Math.Sin(a) * distance;
            ly = s.Y + Math.Cos(a) * distance;
        }
        else
        {
            var b = a + distance / radius;
            lx = s.X + radius * (Math.Cos(a) - Math.Cos(b));
            ly = s.Y + radius * (Math.Sin(b) - Math.Sin(a));
        }

        var t = Math.Abs(s.Length) < 0.000001 ? 0 : distance / s.Length;
        var gradient = (s.GradientStart + (s.GradientEnd - s.GradientStart) * t * 0.5) / 100.0;
        var h = s.Z + distance * gradient;

        return ((float)(ox + lx), (float)h, (float)-(oy + ly));
    }

    private static void AddLine(List<float> d, float ax, float ay, float az, float bx, float by, float bz)
    {
        d.Add(ax); d.Add(ay); d.Add(az);
        d.Add(bx); d.Add(by); d.Add(bz);
    }
}