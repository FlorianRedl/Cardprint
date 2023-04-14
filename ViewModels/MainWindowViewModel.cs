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

namespace Cardprint.ViewModels;


[ObservableObject]
public partial class MainWindowViewModel 
{
    //Settings
    double ViewSize { get { return Settings.Default.ViewSize; } } // 0.1 bis +2
    // int printResolution { get { return Settings.Default.PrintResolution; } } // nötig ?
    string LayoutPath { get { return Settings.Default.LayoutPath; } } // nötig ?
    List<FieldValueAttribute> fieldValueAttributes { get; set; }

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
        //SetViewContent();
    }

    [ObservableProperty]
    public Canvas view;
    [ObservableProperty]
    public Canvas viewBackground;

    
    public MainWindowViewModel()
    {

        printContentHeaders = new List<string>();
        printContentList = new ObservableCollection<PrintContent>();
    }

    public void SetLayouts()
    {
        if (!Directory.Exists(LayoutPath))
        {
            MessageBox.Show("Layout Folder missing!" + "\n \n" + $"Path: {LayoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LayoutNames = new ObservableCollection<string>(XmlReader.GetLayoutNames(LayoutPath));

        if (!LayoutNames.Any()) { MessageBox.Show("No Layouts found!" + "\n \n" + $"Path: {LayoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Information); }
    }


    /// <summary>
    /// ------ BUTTONS ------
    /// </summary>
    /// 
    [RelayCommand]
    private void LoadFromFile()
    {
        MessageBox.Show("[LoadFromFile] not implemented yet");
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
    private void LoadLayout(string layoutName)
    {
        if (string.IsNullOrEmpty(layoutName)) return;

        SelectedLayout = XmlReader.GetLayout(LayoutPath, layoutName);
        string error;
        if (!SelectedLayout.IsValide(out error))
        {
            ClearView();
            ClearPrintContent();
            MessageBox.Show("incorrect layout!" + "\n \n" + $"{error}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        SetView(SelectedLayout);
        SetNewPrintContent(SelectedLayout);
    }

    public void StartUp()
    {
        SetLayouts();
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        fieldValueAttributes = DataAccess.LoadFieldValueAttribute(@$"{appPath}/Data/FieldValueAttributes.xml",out string error);
    }


    /// <summary>
    /// ------ VIEW ------
    /// </summary>
    private void LoadView(LayoutModel layout)
    {
        if (layout == null) return;
        string error;
        if (!layout.IsValide(out error))
        {
            ClearView();
            ClearPrintContent();
            MessageBox.Show("incorrect layout!" + "\n \n" + $"{error}","error",MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        SetView(layout);
        SetNewPrintContent(layout);
    }
    private void SetView(LayoutModel layout)
    {

        ViewBackground = GetViewBackground(layout);
        View = GetCanvas(null, layout);

    }
    
    private void ClearView()
    {
        View = null;
        ViewBackground = null;
        
    }

    /// <summary>
    /// ------ PrintContent ------
    /// </summary>
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


    private Canvas GetViewBackground(LayoutModel layout)
    {
        Canvas canvas = new();

        var width = Calc.MillimeterToPixel(layout.FormatSize.height, ViewSize) ;
        var height = Calc.MillimeterToPixel(layout.FormatSize.width, ViewSize) ;
        canvas.Width = width;
        canvas.Height = height;

        Border border = new Border();
        border.BorderThickness = new System.Windows.Thickness(2);
        border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A2A2A2"));
        border.CornerRadius = new System.Windows.CornerRadius(15);
        border.Width = width;
        border.Height = height;

        canvas.Children.Add(border);
        return canvas;

    }


    /// <summary>
    /// ------ Printing ------
    /// </summary>
    [RelayCommand]
    private void Print()
    {
        PrintHelper.Print();

    }

    private Canvas GetCanvas(PrintContent? printContent, LayoutModel layout)
    {

        var canvas = new Canvas();
        var width = Calc.MillimeterToPixel(layout.FormatSize.height, ViewSize);
        var height = Calc.MillimeterToPixel(layout.FormatSize.width, ViewSize);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width,height)));
        //canvas.Background = new SolidColorBrush(Colors.Beige);

        int fieldIndex = 1;
        foreach (var field in layout.Fields)
        {
            Label label = new Label();


            var lableText = GetFieldText(field, GetPropValue(printContent, $"Field{fieldIndex}"), fieldValueAttributes);


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

    private string GetFieldText(FieldModel field, string printContent, List<FieldValueAttribute> fieldValueAttributes = null)
    {
        if (string.IsNullOrEmpty(field.Value))
        {
            if (string.IsNullOrEmpty(printContent))
            {
                return field.Name;
            }
            return printContent;
        }
        string tempString = new(field.Value);
        if (fieldValueAttributes == null || !fieldValueAttributes.Any()) return tempString;


        foreach (var item in fieldValueAttributes)
        {
            if (field.Value.Contains($"[{item.Key}]"))
            {
                string pattern = $@"\[\b{item.Key}\b\]";
                string replace = item.Value.ToString();
                string result = Regex.Replace(tempString, pattern, replace);
                tempString = result;
            }
        }

        return tempString;
    }

    public static string GetPropValue(PrintContent? pc, string propName)
    {
        if (pc == null) return "";
        var propval = pc.GetType().GetProperty(propName).GetValue(pc, null);

        if(propval != null) return propval.ToString();

        return "";
    }
}
