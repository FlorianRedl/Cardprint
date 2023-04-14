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
using static System.Windows.Forms.DataFormats;
using Format = Cardprint.Models.Format;
using System.Windows.Threading;
using System.Printing;

namespace Cardprint.ViewModels;


internal partial class SettingsViewModel : ObservableValidator
{
    [ObservableProperty]
    public double viewSize;
    [ObservableProperty]
    public double printScale;
    [ObservableProperty]
    public List<string> printers;
    [ObservableProperty]
    public string selectedPrinter;
    [ObservableProperty]
    public string layoutPath;

    [ObservableProperty]
    public Format selectedFormat;
    [ObservableProperty]
    public List<Format> formats;
    [ObservableProperty]
    public string printerStatus;

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
    private DispatcherTimer timer = new DispatcherTimer();

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

        timer.Interval = TimeSpan.FromMilliseconds(250);
        timer.Tick += Timer_Tick; ;
        

        SelectedFormat = Formats.FirstOrDefault();
        printScale = Settings.Default.PrintScale;
        SelectedPrinter = Settings.Default.SelectedPrinter;
        ViewSize = Settings.Default.ViewSize;
        layoutPath = Settings.Default.LayoutPath;
        OffsetX = Settings.Default.PrinterOffsetX; 
        OffsetY = Settings.Default.PrinterOffsetY;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        queue.Refresh();
        var t = queue.IsPrinting;
        var a = queue.IsBusy;
        var b = queue.IsProcessing;
        var c = queue.IsWaiting;
        var n = queue.NumberOfJobs;

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
        timer.Start();
        PrintHelper.PrintTest(SelectedFormat,OffsetX, OffsetY,PrintScale);
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
