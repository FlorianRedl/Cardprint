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

namespace Cardprint.ViewModels;


[ObservableObject]
public partial class MainWindowViewModel 
{
    //Settings
    double viewSize { get { return Settings.Default.ViewSize; } } // 0.1 bis +2
    // int printResolution { get { return Settings.Default.PrintResolution; } } // nötig ?
    string layoutPath { get { return Settings.Default.LayoutPath; } } // nötig ?

    public Action<string[]> OnSelectedLayoutChanges;



    //Layouts
    [ObservableProperty]
    public ObservableCollection<string> layoutNames;
    [ObservableProperty]
    public string selectedLayout;
    [ObservableProperty]
    public ObservableCollection<LayoutModel> layouts;
    partial void OnSelectedLayoutChanging(string? layoutName)
    {
        //LoadView(layout);
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
    partial void OnSelectedPrintContentChanged(PrintContent? layout)
    {
        SetViewContent();
    }

    [ObservableProperty]
    public Canvas view;

    [ObservableProperty]
    public Canvas viewBackground;

    
    public MainWindowViewModel()
    {

        layoutNames = new ObservableCollection<string>(XmlReader.GetLayoutNames(layoutPath));
        //Layouts = new ObservableCollection<LayoutModel>(XmlReader.GetLayouts(layoutPath));
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
       
    }
    [RelayCommand]
    private void AddContent()
    {
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

        Process.Start("explorer.exe",layoutPath);
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


        var width = Utilities.MillimeterToPixel(layout.FormatSize.height, viewSize) ;
        var height = Utilities.MillimeterToPixel(layout.FormatSize.width, viewSize) ;
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

        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        pd.PrintQueue = queue;

        if(pd.ShowDialog() == true)
        {

            //PageMediaSize pageSize = new PageMediaSize(PageMediaSizeName.CreditCard);
            //pd.PrintTicket.PageMediaSize = pageSize;
            //var pv = GetCanvas(SelectedPrintContent ,SelectedLayout);
            //pd.PrintVisual(pv, "printing Card");
        }


    }

    private Canvas GetCanvas(PrintContent? pc, LayoutModel layout)
    {

        var canvas = new Canvas();
        var width = Utilities.MillimeterToPixel(layout.FormatSize.height, viewSize);
        var height = Utilities.MillimeterToPixel(layout.FormatSize.width, viewSize);
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
            var x = Utilities.MillimeterToPixel(field.XCord, viewSize);
            var y = Utilities.MillimeterToPixel(field.YCord, viewSize);
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
