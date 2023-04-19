using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cardprint.Models;
using Cardprint.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using Cardprint.Properties;
using System.Diagnostics;
using System.IO;
using Cardprint.Utilities;
using System.Collections;
using System.Xml;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace Cardprint.ViewModels;


[ObservableObject]
public partial class MainWindowViewModel 
{
    //Settings
    double ViewSize { get { return Settings.Default.ViewSize; } } // 0.1 bis +2
    // int printResolution { get { return Settings.Default.PrintResolution; } } // nötig ?
    string LayoutPath { get { return Settings.Default.LayoutPath; } } // nötig ?

    public Action<string[]> OnSelectedLayoutChanges;



    //Layouts
    [ObservableProperty]
    public ObservableCollection<string> layoutNames;
    [ObservableProperty]
    public string selectedLayoutName;
    [ObservableProperty]
    public LayoutModel selectedLayout;
    [ObservableProperty]
    public ObservableCollection<LayoutModel> layouts;
    partial void OnSelectedLayoutNameChanging(string value)
    {
        LoadLayout(value);
    }

    [ObservableProperty]
    public List<string> printContentHeaders;
    [ObservableProperty]
    public ObservableCollection<PrintContent> printContentList;

    [ObservableProperty]
    public PrintContent selectedPrintContent;
    partial void OnSelectedPrintContentChanged(PrintContent value)
    {
        SetView();
    }
    [ObservableProperty]
    public string printStatus;
    [ObservableProperty]
    public Canvas view;
    [ObservableProperty]
    public Canvas viewBackground;

    private DispatcherTimer timer = new DispatcherTimer();
    public MainWindowViewModel()
    {
        printContentHeaders = new List<string>();
        printContentList = new ObservableCollection<PrintContent>();

        
        timer.Interval = TimeSpan.FromMilliseconds(200);
        timer.Tick += Timer_Tick;
        
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        var printQueue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        printQueue.Refresh();
        var n = printQueue.NumberOfJobs;
        if(n <= 0) 
        {
            PrintStatus = "";
            timer.Stop();
            return;
        };
        PrintStatus = $"Printqueue: {n}";
    }

    /// <summary>
    /// ------ BUTTONS ------
    /// </summary>
    /// 
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
        Process.Start("explorer.exe",LayoutPath);
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
    private void Print()
    {
        timer.Start();

        var result = MessageBox.Show($"Do you want to print {PrintContentList.Count()} Cards?", "Print", MessageBoxButton.YesNo);
        if(result == MessageBoxResult.No) { return; }

        foreach (var item in PrintContentList)
        {
            PrintHelper.Print(CanvasHelper.GetViewCard(GetFieldValues(SelectedPrintContent, SelectedLayout), SelectedLayout, ViewSize));
        }

    }
    private void LoadLayout(string layoutName)
    {
        if (string.IsNullOrEmpty(layoutName)) return;

        SelectedLayout = DataAccess.GetLayout(LayoutPath, layoutName);
        string error;
        if (!SelectedLayout.IsValide(out error))
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
        if (!Directory.Exists(LayoutPath))
        {
            MessageBox.Show("Layout Folder missing!" + "\n \n" + $"Path: {LayoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LayoutNames = new ObservableCollection<string>(DataAccess.GetLayoutNames(LayoutPath));

        if (!LayoutNames.Any()) { MessageBox.Show("No Layouts found!" + "\n \n" + $"Path: {LayoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Information); }
    }

    [RelayCommand]
    private void SetView()
    {
        ViewBackground = CanvasHelper.GetViewBackground(SelectedLayout,ViewSize);
        View = CanvasHelper.GetViewCard(GetFieldValues(SelectedPrintContent, SelectedLayout), SelectedLayout, ViewSize);
    }
    
    private void ClearView()
    {
        View.Children.Clear();
        ViewBackground.Children.Clear();
        
    }

    private void SetNewPrintContent(LayoutModel layout)
    {
        PrintContentList.Clear();
        printContentHeaders = layout.Fields.Where(s=> string.IsNullOrEmpty(s.Value)).Select(s => s.Name).ToList();
        OnSelectedLayoutChanges?.Invoke(printContentHeaders.ToArray());
        PrintContentList.Add(new PrintContent());
    }
    private void ClearPrintContent()
    {
        PrintContentList.Clear();
        OnSelectedLayoutChanges?.Invoke(null);
    }


    private Canvas GetCanvas(PrintContent? printContent, LayoutModel layout)
    {
        if (layout == null) return new Canvas();
        var canvas = new Canvas();
        var width = Calc.MillimeterToPixel(layout.FormatSize.height, ViewSize);
        var height = Calc.MillimeterToPixel(layout.FormatSize.width, ViewSize);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width,height)));

        int fieldIndex = 1;
        foreach (var field in layout.Fields)
        {
            Label label = new Label();

            var lableText = ""; //GetFieldText(field, GetPropValue(printContent, $"Field{fieldIndex}"),out bool isFilled);
            //label.Foreground = isFilled ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.LightGray);
            label.Content = lableText;
            label.FontSize = field.Size;
            label.Padding = new Thickness(0);
            canvas.Children.Add(label);
            var x = Calc.MillimeterToPixel(field.XCord, ViewSize);
            var y = Calc.MillimeterToPixel(field.YCord, ViewSize);
            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            fieldIndex++;
        }

        return canvas;

    }

    private Dictionary<string,string> GetFieldValues(PrintContent printContent,LayoutModel layout)
    {
        Dictionary<string, string> fieldValues = new();

        int fieldIndex = 1;
        foreach (var field in layout.Fields)
        {
            var fieldValue = GetFieldValue(field);
            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValues.Add(field.Name, fieldValue);
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


    private string GetFieldValue(FieldModel field)
    {
        string tempValue = string.Empty;
        if (string.IsNullOrEmpty(field.Value)) return tempValue;

        if (field.Value.Contains($"[date]"))
        {
            var pattern = $@"\[\bdate\b\]";
            var replace = DateTime.Now.ToString("dd.MM.yyyy");
            string result = Regex.Replace(field.Value, pattern, replace);
            tempValue = result;
        }

        return tempValue;
    }


    public static string GetPropValueFromPrintContent(PrintContent pc, string propName)
    {
        if (pc == null) return "";
        var propval = pc.GetType().GetProperty(propName).GetValue(pc, null);
        if(propval != null) return propval.ToString();
        return "";
    }
    public void StartUp()
    {
        SetLayouts();
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
    }
}
