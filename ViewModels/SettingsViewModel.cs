using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardprint.Properties;
using Cardprint.Views;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Cardprint.Utilities;
using Cardprint.Models;
using Format = Cardprint.Models.Format;
using System.Windows.Threading;
using System.Printing;
using System.Windows;

namespace Cardprint.ViewModels;


internal partial class SettingsViewModel : ObservableValidator
{
    [ObservableProperty]
    public double viewSize = new();
    [ObservableProperty]
    public double printScale = new() ;
    [ObservableProperty]
    public List<string> printers = new();
    [ObservableProperty]
    public string selectedPrinter = string.Empty;
    [ObservableProperty]
    public string layoutPath = string.Empty;

    [ObservableProperty]
    public Format? selectedFormat;
    [ObservableProperty]
    public List<Format> formats = new();
    [ObservableProperty]
    public string printerStatus = string.Empty;

    private double offsetX;
    [Range(-20,20)]
    public double OffsetX
    {
        get => offsetX;
        set => SetProperty(ref offsetX, value, true);
    }

    private double offsetY;
    [Range(-20, 20)]
    public double OffsetY
    {
        get => offsetY;
        set => SetProperty(ref offsetY, value, true);
    }
    private DispatcherTimer _timer;

    public SettingsViewModel()
    {
        var p = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
        Printers = p.Cast<string>().ToList();

        Formats = new List<Format>
        {
            new Format("ID-1", 85.60, 53.98),
            new Format("ID-2", 105.00, 74.00),
            new Format("ID-3", 125.00, 88.00),
            new Format("ID-000", 25.00, 15.00),
            new Format("A4", 210.00, 297.00)

        };
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick += Timer_Tick; ;
        

        SelectedFormat = Formats.First();
        printScale = Settings.Default.PrintScale;
        SelectedPrinter = Settings.Default.SelectedPrinter;
        ViewSize = Settings.Default.ViewSize;
        layoutPath = Settings.Default.LayoutPath;
        OffsetX = Settings.Default.PrinterOffsetX; 
        OffsetY = Settings.Default.PrinterOffsetY;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (SelectedPrinter == string.Empty) return;
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(SelectedPrinter);
        queue.Refresh();
        var t = queue.IsPrinting;
        var a = queue.IsBusy;
        var b = queue.IsProcessing;
        var c = queue.IsWaiting;
        var n = queue.NumberOfJobs;
        var x = queue.QueueStatus;

        PrinterStatus = $"PrintQueue Items: {n}";

    }

    [RelayCommand]
    private void Save(object obj)
    {
        Settings.Default.PrintScale = PrintScale;
        Settings.Default.ViewSize = ViewSize;
        Settings.Default.SelectedPrinter = SelectedPrinter;
        Settings.Default.LayoutPath = layoutPath;
        Settings.Default.PrinterOffsetX = OffsetX;
        Settings.Default.PrinterOffsetY = OffsetY;
        Settings.Default.Save();
        var win = obj as SettingsView;
        win?.Close();
        
    }

    [RelayCommand]
    private void TestPrint()
    {
        if (string.IsNullOrEmpty(SelectedPrinter)) { MessageBox.Show("No Printer selected!", "error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        _timer.Start();
        PrintHelper.Print(CanvasHelper.GetTestPrintCanvas(SelectedFormat!,OffsetX, OffsetY,PrintScale), SelectedPrinter,1);
    }

    [RelayCommand]
    private void PickFile()
    {
        System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
 
        if (Directory.Exists(layoutPath)) 
        {
            openFileDlg.InitialDirectory = layoutPath;
        }
        else
        {
            openFileDlg.InitialDirectory = @"C:\";
        }
        var result = openFileDlg.ShowDialog();

        if (result.ToString() != string.Empty && result == System.Windows.Forms.DialogResult.OK)
        {
            LayoutPath =  openFileDlg.SelectedPath;
        }
    }

}
