using LiveChartsCore;
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
        LiveCharts.Configure(config => config.HasMap<(int, string)>((point, index) => new(index, point.Item1)));
    }
}
