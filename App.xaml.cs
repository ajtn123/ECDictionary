using SQLitePCL;
using System.Windows;

namespace ECDictionary;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Batteries.Init();
    }
}
