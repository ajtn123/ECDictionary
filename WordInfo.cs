using System.Windows;

namespace ECDictionary;

public class WordInfo
{
    public WordInfo(object[] infos, Dictionary<string, Visibility?>? visibilities = null)
    {
        foreach (var prop in MainWindow.TableSchema)
            SetContent(prop.Key, infos[prop.Value]);
        Visibilities = (visibilities ?? DefaultVisibilities).ToDictionary(p => p.Key, p => p.Value ?? GetVisibility(GetContent(p.Key)));

        if (Pos != null)
            Pos = string.Join("   ", Pos.Split('/').Select(s => $"{PosNotations[s.Split(':')[0]]}: {s.Split(':')[1]}%"));

        if (Exchange != null)
            Exchange = Exchange.Replace("/", "   ").Replace(":", ": ");
    }

    public Dictionary<string, Visibility> Visibilities { get; set; }

    public string? Word { get; set; }
    public string? Sw { get; set; }
    public string? Phonetic { get; set; }
    public string? Definition { get; set; }
    public string? Translation { get; set; }
    public string? Pos { get; set; }
    public long? Bnc { get; set; }
    public long? Frq { get; set; }
    public string? Exchange { get; set; }

    private static Visibility GetVisibility(object? content)
        => !string.IsNullOrWhiteSpace(content?.ToString()) ? Visibility.Visible : Visibility.Collapsed;

    public static Dictionary<string, Visibility?> DefaultVisibilities => new()
    {
        ["word"] = null,
        ["sw"] = Visibility.Collapsed,
        ["phonetic"] = null,
        ["definition"] = null,
        ["translation"] = null,
        ["pos"] = null,
        ["bnc"] = null,
        ["frq"] = null,
        ["exchange"] = null,
    };
    private static Dictionary<string, string> PosNotations => new()
    {
        ["a"] = "article",
        ["c"] = "conjunction",
        ["d"] = "determiner",
        ["i"] = "preposition",
        ["j"] = "adjective",
        ["m"] = "numeral",
        ["n"] = "noun",
        ["p"] = "pronoun",
        ["r"] = "adverb",
        ["t"] = "infinitival marker",
        ["u"] = "interjection",
        ["v"] = "verb",
    };

    public object? GetContent(string name) => name switch
    {
        "word" => Word,
        "sw" => Sw,
        "phonetic" => Phonetic,
        "definition" => Definition,
        "translation" => Translation,
        "pos" => Pos,
        "bnc" => Bnc,
        "frq" => Frq,
        "exchange" => Exchange,
        _ => null,
    };
    public void SetContent(string name, object? value)
    {
        if (value is DBNull) value = null;
        switch (name)
        {
            case "word":
                Word = value?.ToString(); break;
            case "sw":
                Sw = value?.ToString(); break;
            case "phonetic":
                Phonetic = value?.ToString(); break;
            case "definition":
                Definition = value?.ToString(); break;
            case "translation":
                Translation = value?.ToString(); break;
            case "pos":
                Pos = value?.ToString(); break;
            case "bnc":
                Bnc = (long?)value; break;
            case "frq":
                Frq = (long?)value; break;
            case "exchange":
                Exchange = value?.ToString(); break;
            default: break;
        }
    }
}
