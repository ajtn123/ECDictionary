using System.Windows;

namespace ECDictionary;

public class WordInfo
{
    public WordInfo(object[] infos, bool?[]? visibilities = null)
    {
        Contents = [.. infos.Select(x => x.ToString())];
        Visibilities = [.. (visibilities ??= new bool?[Contents.Length]).Select((v, i) => GetVisibility(v, Contents[i]))];
    }

    public string?[] Contents { get; set; }
    public Visibility[] Visibilities { get; set; }

    private static Visibility GetVisibility(bool? visibility, string? content)
    {
        if (visibility == null) return GetVisibility(!string.IsNullOrWhiteSpace(content));
        else return GetVisibility((bool)visibility);
    }
    private static Visibility GetVisibility(bool visibility)
        => visibility ? Visibility.Visible : Visibility.Collapsed;
}
