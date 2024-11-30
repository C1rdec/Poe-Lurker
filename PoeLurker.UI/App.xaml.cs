using System.Windows;
using Velopack;

namespace PoeLurker.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        VelopackApp.Build().Run();

        InitializeComponent();
    }
}
