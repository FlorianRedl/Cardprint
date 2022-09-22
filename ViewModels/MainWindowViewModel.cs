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
    int ViewSize { get { return Settings.Default.Resolution; } }

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
    private void Print()
    {

        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue("Microsoft Print to PDF");
        pd.PrintQueue = queue;
        pd.PrintVisual(Canvas, "printing canvas");


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

        var width = 3235 / ViewSize;
        var height = 2022 / ViewSize;
        Canvas.Width = width;
        Canvas.Height = height;

        foreach (var field in layout.Fields)
        {
            Label label = new Label();
            label.Content = field.Name;
            label.FontSize = field.Size;
            Canvas.Children.Add(label);

            Canvas.SetTop(label, field.YCord * ViewSize);
            Canvas.SetLeft(label, field.XCord * ViewSize);
        }
    }

    private void LoadViewBackground(LayoutModel layout)
    {
        ViewBackground.Children.Clear();

        var width = 3235 / ViewSize;
        var height = 2022 / ViewSize;
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
    
}
