using Omsi64.Engine.Omsi;
using Omsi64.Engine.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Omsi64.Engine;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        try
        {
            var noViewer = args.Any(x => x.Equals("--no-viewer", StringComparison.OrdinalIgnoreCase));
            var inputArgument = args.FirstOrDefault(x => !x.StartsWith("--", StringComparison.Ordinal));

            if (inputArgument is null)
            {
                PrintUsage();
                return;
            }

            var inputPath = Path.GetFullPath(inputArgument.Trim('"'));
            var globalCfgPath = Directory.Exists(inputPath)
                ? Path.Combine(inputPath, "global.cfg")
                : inputPath;

            if (!File.Exists(globalCfgPath))
            {
                Console.Error.WriteLine($"Nie znaleziono pliku: {globalCfgPath}");
                return;
            }

            var map = GlobalConfigParser.Load(globalCfgPath);

            if (map.Tiles.Count == 0)
            {
                Console.Error.WriteLine("Nie znaleziono żadnych sekcji [map] w global.cfg.");
                return;
            }

            var loaded = MapContentLoader.Load(map);
            PrintSummary(loaded);

            if (noViewer)
                return;

            var native = new NativeWindowSettings
            {
                Size = new Vector2i(1280, 720),
                Title = $"OMSI64 - Milestone 2 - {map.Name}"
            };

            using var viewer = new MapViewerWindow(GameWindowSettings.Default, native, loaded);
            viewer.Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Błąd podczas uruchamiania OMSI64:");
            Console.Error.WriteLine(ex);
        }
    }

    private static void PrintSummary(OmsiLoadedMap loaded)
    {
        var objectCount = loaded.TileContents.Sum(x => x.Objects.Count);
        var splineCount = loaded.TileContents.Sum(x => x.Splines.Count);
        var warnings = loaded.TileContents.SelectMany(x => x.Warnings).ToArray();

        Console.WriteLine($"Mapa: {loaded.Map.Name}");
        Console.WriteLine($"Folder: {loaded.Map.MapDirectory}");
        Console.WriteLine($"Kafelki: {loaded.Map.Tiles.Count}");
        Console.WriteLine($"Obiekty [object]: {objectCount}");
        Console.WriteLine($"Spliny [spline]/[spline_h]: {splineCount}");
        Console.WriteLine($"Brakujące assety: {loaded.MissingAssets.Count}");
        Console.WriteLine($"Ostrzeżenia parsera: {warnings.Length}");
    }

    private static void PrintUsage()
    {
        Console.WriteLine("OMSI64 - Milestone 2");
        Console.WriteLine();
        Console.WriteLine("Użycie:");
        Console.WriteLine("  Omsi64.Engine.exe \"C:\\Program Files (x86)\\Steam\\steamapps\\common\\OMSI 2\\maps\\Grundorf\"");
        Console.WriteLine("  Omsi64.Engine.exe --no-viewer \"D:\\OMSI 2\\maps\\Grundorf\"");
    }
}