using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardprint.Properties;
using Cardprint.Views;


namespace Cardprint.ViewModels;

[ObservableObject]
internal partial class SettingsViewModel
{
    [ObservableProperty]
    public double viewSize;
    [ObservableProperty]
    public int printResolution;
    [ObservableProperty]
    public List<string> printers;
    [ObservableProperty]
    public string selectedPrinter;
    [ObservableProperty]
    public string layoutPath;
    [ObservableProperty]
    public double offsetX;
    [ObservableProperty]
    public double offsetY;

    public SettingsViewModel()
    {
        var p = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
        Printers = p.Cast<string>().ToList();

        printResolution = Settings.Default.PrintResolution;
        SelectedPrinter = Settings.Default.SelectedPrinter;
        ViewSize = Settings.Default.ViewSize;
        layoutPath = Settings.Default.LayoutPath;
        OffsetX = Settings.Default.PrinterOffsetX; 
        offsetY = Settings.Default.PrinterOffsetY;
    }

    [RelayCommand]
    private void Save(object obj)
    {
        Settings.Default.PrintResolution = PrintResolution;
        Settings.Default.ViewSize = ViewSize;
        Settings.Default.SelectedPrinter = SelectedPrinter;
        Settings.Default.LayoutPath = layoutPath;
        Settings.Default.PrinterOffsetX = offsetX;
        Settings.Default.PrinterOffsetY = offsetY;
        Settings.Default.Save();
        var win = obj as SettingsView;
        win?.Close();
        
    }

}
