using Cardprint.Models;
using Cardprint.Properties;
using Cardprint.Utilities;
using Cardprint.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Cardprint.ViewModels;


[ObservableObject]
public partial class MainWindowViewModel 
{
    //Settings
    double _viewSize { get { return Settings.Default.ViewSize; } } // 0.1 bis +2
    string _layoutPath { get { return Settings.Default.LayoutPath; } } // nötig ?
    string _selectedPrinter { get { return Settings.Default.SelectedPrinter; } }
    PrintQueue? _selectedPrintQueue { get { return Utils.GetPrintQueueFromName(Settings.Default.SelectedPrinter); } }
    public Action<string[]?>? OnSelectedLayoutChanges;


    [ObservableProperty]
    public string appVersion = string.Empty;
    //Layouts
    [ObservableProperty]
    public ObservableCollection<string> layoutNames = new();
    [ObservableProperty]
    public string? selectedLayoutName;
    partial void OnSelectedLayoutNameChanging(string? value)
    {
        if(string.IsNullOrEmpty(value)) return;
        if(PrintContentList.Count > 1)
        {
            //Todo Dialog the already filled fields will be lost
        }

        LoadLayout(value);
    }
    [ObservableProperty]
    public Layout? selectedLayout;
    [ObservableProperty]
    public ObservableCollection<Layout> layouts = new();

    [ObservableProperty]
    public List<string> printContentHeaders = new();
    [ObservableProperty]
    public ObservableCollection<PrintContent> printContentList = new();

    [ObservableProperty]
    public PrintContent selectedPrintContent = new();
    partial void OnSelectedPrintContentChanged(PrintContent value)
    {
        SetView();
    }
    [ObservableProperty]
    public string printStatus = string.Empty;
    [ObservableProperty]
    public Canvas view = new();
    [ObservableProperty]
    public Canvas viewBackground = new();

    private DispatcherTimer _printerCheckTimer = new();
    public MainWindowViewModel()
    {
        _printerCheckTimer.Interval = TimeSpan.FromMilliseconds(250);
        _printerCheckTimer.Tick += Timer_Tick;
        _printerCheckTimer.Start();
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        if (version != null) AppVersion = $"Version {version.ToString(3)}";
    }

    [RelayCommand]
    private void LoadFromFile()
    {
        MessageBox.Show("[LoadFromFile] not implemented!");
    }
    [RelayCommand]
    private void AddContent()
    {
        if (selectedLayout == null) return;
        PrintContentList.Add(new PrintContent());
    }
    [RelayCommand]
    private void RemoveContent()
    {
        if (PrintContentList.Count <=1) return;

        if(SelectedPrintContent != null)
        {
            PrintContentList.Remove(SelectedPrintContent);
            return;
        }
        else
        {
            var item = PrintContentList.LastOrDefault();
            if (item != null) PrintContentList.Remove(item);
        }

    }
    [RelayCommand]
    private void OpenSettings()
    {
        var win = new SettingsView();
        win.Owner = Application.Current.MainWindow;
        win.Show();
        win.Closed += Settings_Closed;
    }

    private void Settings_Closed(object? sender, EventArgs e)
    {
        SetLayouts();

        
    }

    [RelayCommand]
    private void OpenLayoutFolder()
    {
        Process.Start("explorer.exe",_layoutPath);
    }
    [RelayCommand]
    private void OpenGithubCardPrint()
    {
        var url ="https://github.com/FlorianRedl/Cardprint";
        Process.Start("explorer.exe", url);
    }
    [RelayCommand]
    private void OpenFlrStudios()
    {
        var url = "https://www.flr-studios.at";
        Process.Start("explorer.exe", url);
    }
    [RelayCommand]
    private void ExitApp()
    {
        Application.Current.Shutdown();
    }
    [RelayCommand]
    private void Print()
    {
        if(_selectedPrintQueue is null) { MessageBox.Show("No Printer selected!", "error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        if(SelectedLayout == null) { MessageBox.Show("No Layout selected!", "error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        var pageCount = PrintContentList.Sum(s => s.Quantity);
        var result = MessageBox.Show($"Do you want to print {pageCount} Cards?", "Print", MessageBoxButton.YesNo);
        if(result == MessageBoxResult.No) { return; }

        foreach (var item in PrintContentList)
        {
            var fieldValues = GetFieldValues(item, SelectedLayout); 
            var canvas = CanvasHelper.GetCanvas(fieldValues,SelectedLayout, Settings.Default.PrintScale, false);
            PrintHelper.Print(canvas, _selectedPrintQueue, item.Quantity);
        }

    }
    private void LoadLayout(string layoutName)
    {
        if (string.IsNullOrEmpty(layoutName)) return;

        string error;
        SelectedLayout = DataAccess.LoadLayout(_layoutPath, layoutName, out error);
        //var layout = DataAccess.LoadLayout(_layoutPath, layoutName, out error);


        if (SelectedLayout is null)
        {
            ClearView();
            ClearPrintContent();
            MessageBox.Show("incorrect layout!" + "\n \n" + $"{error}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        SetView();
        SetNewPrintContent(SelectedLayout);
    }

    public void SetLayouts()
    {
        if (!Directory.Exists(_layoutPath))
        {
            MessageBox.Show("Layout Folder missing!" + "\n \n" + $"Path: {_layoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LayoutNames = new ObservableCollection<string>(DataAccess.GetLayoutNames(_layoutPath));

        if (!LayoutNames.Any()) { MessageBox.Show("No Layouts found!" + "\n \n" + $"Path: {_layoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Information); }
    }

    [RelayCommand]
    private void SetView()
    {
        if(SelectedLayout == null) return;
        ViewBackground = CanvasHelper.GetViewBackground(SelectedLayout,_viewSize);
        var fieldValues = GetFieldValues(SelectedPrintContent, SelectedLayout);
        View = CanvasHelper.GetCanvas(fieldValues, SelectedLayout, _viewSize,true);
    }
    
    private void ClearView()
    {
        if(View == null || ViewBackground == null) return;
        View.Children.Clear();
        ViewBackground.Children.Clear();
    }

    private void SetNewPrintContent(Layout layout)
    {
        PrintContentList.Clear();

        var textfields =  layout.TextFields;

        PrintContentHeaders = textfields.Where(s=> s.GetType() == typeof(TextField) & string.IsNullOrEmpty(s.Text)).Select(s => s.Name).ToList();
        OnSelectedLayoutChanges?.Invoke(printContentHeaders.ToArray());
        PrintContentList.Add(new PrintContent());
    }
    private void ClearPrintContent()
    {
        PrintContentList.Clear();
        OnSelectedLayoutChanges?.Invoke(null);
    }

    private Dictionary<string,string> GetFieldValues(PrintContent printContent,Layout layout)
    {
        Dictionary<string, string> fieldValues = new();

        int fieldIndex = 1;
        foreach (var field in layout.TextFields)
        {
            var fieldValue = FieldValueAttributeHandler.CheckAndReplaceTextValue(field);

            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValues.Add(field.Name, fieldValue);
                continue;
            }

            var propValue = GetPropValueFromPrintContent(printContent,$"Field{fieldIndex}");
            if (!string.IsNullOrEmpty(propValue)) 
            {
                fieldValues.Add(field.Name, propValue);
            }


            fieldIndex++;
        }
        
        return fieldValues;
    }


    public static string GetPropValueFromPrintContent(PrintContent pc, string propName)
    {
        if (pc == null) return "";
        var propval = pc.GetType().GetProperty(propName)?.GetValue(pc, null);
        if (propval == null) return "";
        return propval.ToString()!;
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Settings.Default.SelectedPrinter)) return;

        try
        {
            if(_selectedPrintQueue == null)
            {
                return;
            };


            _selectedPrintQueue.Refresh();
            var n = _selectedPrintQueue.NumberOfJobs;
            if (n <= 0)
            {
                PrintStatus = "";
                return;
            };
            PrintStatus = $"Printqueue {_selectedPrintQueue.Name}: {n}";

        }
        catch (Exception)
        {

        }
    }

    public void StartUp()
    {
        SetLayouts();
    }
}
