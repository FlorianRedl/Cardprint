using System.Windows;
using Cardprint.Views;

namespace Cardprint;

public partial class App : Application
{
    public App()
    {
        var appWindow = new MainWindowView();
        appWindow.Show();

    }
    
}
