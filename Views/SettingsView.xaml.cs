using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cardprint.ViewModels;

namespace Cardprint.Views;


public partial class SettingsView : Window
{

    private SettingsViewModel SettingsViewModel { get; }
    public SettingsView()
    {
        DataContext = SettingsViewModel = new SettingsViewModel();
        InitializeComponent();
    }
}
