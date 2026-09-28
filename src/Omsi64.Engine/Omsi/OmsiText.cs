using System.Text;

namespace Omsi64.Engine.Omsi;

internal static class OmsiText
{
    public static string ReadAllText(string path) => Decode(File.ReadAllBytes(path));

    public static string[] ReadMeaningfulLines(string path)
    {
        var text = ReadAllText(path);
        return text.Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .Where(x => !x.StartsWith("//", StringComparison.Ordinal) && !x.StartsWith(";", StringComparison.Ordinal))
            .ToArray();
    }

    private static string Decode(byte[] bytes)
    {
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return Encoding.Unicode.GetString(bytes);
        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return Encoding.BigEndianUnicode.GetString(bytes);
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes);

        var sampleLength = Math.Min(bytes.Length, 4096);
        var zeroCount = 0;
        for (var i = 0; i < sampleLength; i++)
            if (bytes[i] == 0) zeroCount++;

        if (sampleLength > 0 && zeroCount > sampleLength / 8)
            return Encoding.Unicode.GetString(bytes);

        return Encoding.Latin1.GetString(bytes);
    }
}