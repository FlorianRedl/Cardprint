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



namespace Cardprint.ViewModels;


[ObservableObject]
public partial class MainWindowViewModel 
{
    int viewSize { get { return Settings.Default.ViewSize; } }
    int printResolution { get { return Settings.Default.PrintResolution; } }

    public Action<string[]> OnSelectedLayoutChanges;




    //Layouts
    [ObservableProperty]
    public ObservableCollection<LayoutModel> layouts;
    [ObservableProperty]
    public LayoutModel selectedLayout;
    partial void OnSelectedLayoutChanging(LayoutModel? layout)
    {
        SetView(layout);
        SetNewPrintContent(layout);
    }

    private void SetNewPrintContent(LayoutModel layout)
    {
        PrintContentList.Clear();
        printContentHeaders = layout.Fields.Select(s => s.Name).ToList();
        OnSelectedLayoutChanges?.Invoke(printContentHeaders.ToArray());
        PrintContentList.Add(new PrintContent());
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
    partial void OnSelectedPrintContentChanging(PrintContent? layout)
    {
      

    }

    [ObservableProperty]
    public Canvas canvas;

    [ObservableProperty]
    public Canvas viewBackground;

    
    public MainWindowViewModel()
    {
        
        Layouts = new ObservableCollection<LayoutModel>(XmlReader.GetLayouts());
        //Printers
        var s = System.Drawing.Printing.PrinterSettings.InstalledPrinters;

        Canvas = new Canvas();
        ViewBackground = new Canvas();
        printContentHeaders = new List<string>();
        printContentList = new ObservableCollection<PrintContent>(DataAccess.GetPrintContentToLayout());


    }

    

    
    [RelayCommand]
    private void SelectedItemChanged()
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


    private void SetView(LayoutModel layout)
    {

        LoadViewBackground(layout);

        Canvas.Children.Clear();

        var width = 3235 / viewSize;
        var height = 2022 / viewSize;
        Canvas.Width = width;
        Canvas.Height = height;

        foreach (var field in layout.Fields)
        {
            Label label = new Label();
            label.Content = field.Name;
            label.FontSize = field.Size;
            Canvas.Children.Add(label);

            Canvas.SetLeft(label, field.XCord * viewSize);
            Canvas.SetTop(label, field.YCord * viewSize);
        }
    }

    private void LoadViewBackground(LayoutModel layout)
    {
        ViewBackground.Children.Clear();

        var width = 3235 / viewSize;
        var height = 2022 / viewSize;
        ViewBackground.Width = width;
        ViewBackground.Height = height;
        

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
        ViewBackground.Children.Add(border);


    }


    //----- Printing -----
    [RelayCommand]
    private void Print()
    {

        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        pd.PrintQueue = queue;

        //PageMediaSize pageSize = new PageMediaSize(PageMediaSizeName.CreditCard);
        //pd.PrintTicket.PageMediaSize = pageSize;
        var pv = GetPrintCanvas(SelectedPrintContent ,SelectedLayout);
        pd.PrintVisual(pv, "printing Card");


    }

    private Canvas GetPrintCanvas(PrintContent pc, LayoutModel layout)
    {

        var canvas = new Canvas();;
        var width = 3235 / printResolution;
        var height = 2040 / printResolution;
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width,height)));        

     

        int fieldIndex = 1;
        foreach (var field in layout.Fields)
        {
            Label label = new Label();
            var name = GetPropValue(pc, $"Field{fieldIndex}");
            label.Content = name;
            label.FontSize = field.Size;
            canvas.Children.Add(label);
            Canvas.SetTop(label, field.YCord * printResolution);
            Canvas.SetLeft(label, field.XCord * printResolution);
            fieldIndex++;
        }

        return canvas;

    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }
}
