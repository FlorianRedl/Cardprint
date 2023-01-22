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
    partial void OnSelectedLayoutNameChanging(string? value)
    {
        LoadLayout(value);
    }

    // PrintContent
    [ObservableProperty]
    public float selectedIndex = 1;
    [ObservableProperty]
    public List<PrintContent> selectedPrintContents;
    [ObservableProperty]
    public List<string> printContentHeaders;
    [ObservableProperty]
    public ObservableCollection<PrintContent> printContentList;
    [ObservableProperty]
    public PrintContent selectedPrintContent;
    partial void OnSelectedPrintContentChanged(PrintContent? value)
    {
        SetViewContent();
    }

    [ObservableProperty]
    public Canvas view;
    [ObservableProperty]
    public Canvas viewBackground;

    
    public MainWindowViewModel()
    {
        if (Directory.Exists(LayoutPath))
        {
            LayoutNames = new ObservableCollection<string>(XmlReader.GetLayoutNames(LayoutPath));
        }
        else
        {
            MessageBox.Show("Layout Folder missing!" + "\n \n" + $"Path: {LayoutPath}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        printContentHeaders = new List<string>();
        printContentList = new ObservableCollection<PrintContent>(DataAccess.GetPrintContentToLayout());

    }

    /// <summary>
    /// ------ BUTTONS ------
    /// </summary>
    /// 
    [RelayCommand]
    private void LoadFromFile()
    {
        MessageBox.Show("[LoadFromFile] not implemented");
    }
    [RelayCommand]
    private void AddContent()
    {
        if (string.IsNullOrEmpty(SelectedLayoutName)) return;
        PrintContentList.Add(new PrintContent());
    }
    [RelayCommand]
    private void RemoveContent()
    {
        if (PrintContentList.Count <=1) return;
        SelectedPrintContent = null;
        var item = PrintContentList.LastOrDefault();
        if (item != null) PrintContentList.Remove(item);

    }
    [RelayCommand]
    private void OpenSettings()
    {
        var win = new SettingsView();
        win.Show();
    }
    [RelayCommand]
    private void OpenLayoutFolder()
    {

        Process.Start("explorer.exe",LayoutPath);
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
    private void SetViewContent()
    {

        //View = GetCanvas(selectedPrintContent, selectedLayout);
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
        printContentHeaders = layout.Fields.Select(s => s.Name).ToList();
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


        if (layout.BackgroundImg != null)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(layout.BackgroundImg));
            border.Child = img;
        }
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

        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        pd.PrintQueue = queue;
        PageMediaSize pSize = new PageMediaSize(PageMediaSizeName.CreditCard);
        pd.PrintTicket.PageMediaSize = pSize;

        //Size pageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
        //pCanvas.Measure(pageSize);
        //pCanvas.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
        var pCanvas = GetCanvas(SelectedPrintContent, SelectedLayout);
        pd.PrintVisual(pCanvas, "printing Card");

    }

    private Canvas GetCanvas(PrintContent? pc, LayoutModel layout)
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
            string lableText = "";
            if(pc != null)
            {
                var t = GetPropValue(pc, $"Field{fieldIndex}");
                if (t != null) lableText = t.ToString();
            }
            else
            {
                lableText = field.Name;
            }

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

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }
}
