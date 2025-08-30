using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace ECDictionary;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SqliteConnection connection = new($"Data Source={Config.DbPath};Mode=ReadOnly");
    private readonly ObservableCollection<WordInfo> words = [];
    public static Dictionary<string, int> TableSchema { get; } = [];
    public MainWindow()
    {
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info({Config.TableName});";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            TableSchema.Add(reader.GetString(1), reader.GetInt32(0));

        InitializeComponent();
        SwResults.ItemsSource = words;
        SearchBox.TextChanged += (_, _) =>
        {
            var sw = Sw;
            if (sw == currentSw)
            {
                SwResults.Visibility = Visibility.Visible;
                MoreButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SwResults.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(sw)) MoreButton.Visibility = Visibility.Visible;
                else MoreButton.Visibility = Visibility.Collapsed;
            }

            if (Word != currentWord)
                Match();
        };
        MoreButton.Click += (_, _) => _ = Search();
        KeyDown += (_, e) => { if (e.Key == System.Windows.Input.Key.Enter) _ = Search(); };
    }

    private string? currentWord;
    private void Match()
    {
        var word = Word;
        currentWord = word;

        if (string.IsNullOrEmpty(word)) { WordResultContainer.Visibility = Visibility.Collapsed; return; }

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {Config.TableName} WHERE word = @word";
        cmd.Parameters.AddWithValue("@word", word);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            object[] infos = new object[reader.FieldCount];
            reader.GetValues(infos);
            var v = WordInfo.DefaultVisibilities;
            v["word"] = Visibility.Collapsed;
            WordResult.Content = new WordInfo(infos, v);

            WordResultContainer.Visibility = Visibility.Visible;
        }
        else WordResultContainer.Visibility = Visibility.Collapsed;
    }


    private string? currentSw;
    private async Task Search()
    {
        SwResults.Visibility = Visibility.Visible;
        MoreButton.Visibility = Visibility.Collapsed;

        var sw = Sw;
        if (sw == currentSw) return;

        currentSw = sw;
        words.Clear();

        if (string.IsNullOrEmpty(sw)) return;

        using var cmd = connection.CreateCommand();
        if (CJK().IsMatch(Word ?? ""))
        {
            cmd.CommandText = $"SELECT * FROM {Config.TableName} WHERE translation LIKE @sw";
            cmd.Parameters.AddWithValue("@sw", $"%{Word}%");
        }
        else
        {
            cmd.CommandText = $"SELECT * FROM {Config.TableName} WHERE sw = @sw";
            cmd.Parameters.AddWithValue("@sw", sw);
        }

        using var reader = await cmd.ExecuteReaderAsync();
        while (reader.Read())
        {
            object[] infos = new object[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                infos[i] = reader.GetValue(i);
            words.Add(new WordInfo(infos));
        }
    }

    private string? Word => SearchBox.Text.Trim();
    private string? Sw => Word != null ? new string([.. Word.Where(c => !char.IsPunctuation(c) && !char.IsSeparator(c))]).ToLower() : null;

    [GeneratedRegex(@"\p{IsCJKUnifiedIdeographsExtensionA}|\p{IsCJKUnifiedIdeographs}")]
    private static partial Regex CJK();
}