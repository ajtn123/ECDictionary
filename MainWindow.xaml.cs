using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Windows;

namespace ECDictionary;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SqliteConnection conn = new($"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\tools\\ecdict.db;Mode=ReadOnly");
    private readonly ObservableCollection<WordInfo> words = [];
    public MainWindow()
    {
        conn.Open();
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

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM stardict WHERE word = @word";
        cmd.Parameters.AddWithValue("@word", word);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            object[] infos = new object[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                infos[i] = reader.GetValue(i);
            bool?[] v = new bool?[reader.FieldCount];
            v[0] = false; v[1] = false;
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

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM stardict WHERE sw = @sw";
        cmd.Parameters.AddWithValue("@sw", sw);

        using var reader = await cmd.ExecuteReaderAsync();
        while (reader.Read())
        {
            object[] infos = new object[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                infos[i] = reader.GetValue(i);
            bool?[] v = new bool?[reader.FieldCount];
            v[1] = false;
            words.Add(new WordInfo(infos, v));
        }
    }

    private string? Word => SearchBox.Text.Trim();
    private string? Sw => Word != null ? new string([.. Word.Where(c => !char.IsPunctuation(c) && !char.IsSeparator(c))]).ToLower() : null;
}