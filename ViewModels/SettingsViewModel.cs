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
    partial void OnSelectedPrinterChanging(string value)
    {
        _selectedPrintQueue = Utils.GetPrintQueueFromName(value);
    }
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
    private PrintQueue? _selectedPrintQueue;

    public SettingsViewModel()
    {
        Printers = GetPrinterNames();

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
        _selectedPrintQueue = Utils.GetPrintQueueFromName(SelectedPrinter);
        ViewSize = Settings.Default.ViewSize;
        layoutPath = Settings.Default.LayoutPath;
        OffsetX = Settings.Default.PrinterOffsetX; 
        OffsetY = Settings.Default.PrinterOffsetY;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_selectedPrintQueue is null) return;

        try
        {
            if (_selectedPrintQueue == null)
            {
                return;
            };


            _selectedPrintQueue.Refresh();
            var n = _selectedPrintQueue.NumberOfJobs;
            if (n <= 0)
            {
                PrinterStatus = "";
                return;
            };
            PrinterStatus = $"Printqueue {_selectedPrintQueue.Name}: {n}";

        }
        catch (Exception)
        {

        }

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
        if (_selectedPrintQueue is null) { MessageBox.Show("Printer not found!", "error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        PrintHelper.Print(CanvasHelper.GetTestPrintCanvas(SelectedFormat!,OffsetX, OffsetY,PrintScale), _selectedPrintQueue, 1);
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


    private List<string> GetPrinterNames()
    {
        var printerNames = new List<string>();
        try
        {
            var localServer = new LocalPrintServer();
            var localPrintQueues = localServer.GetPrintQueues();
            printerNames.AddRange(localPrintQueues.Select(s => s.Name));

            var server = new PrintServer();
            var printQueues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Connections });
            printerNames.AddRange(printQueues.Select(s => s.Name));
            return printerNames;

        }
        catch (Exception)
        {
            MessageBox.Show("Error getting Printer List", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return printerNames;
        }
    }

}
