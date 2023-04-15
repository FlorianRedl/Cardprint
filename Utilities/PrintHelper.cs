using Cardprint.Models;
using Cardprint.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Cardprint.Utilities;

static public class PrintHelper
{



    public static void Print()
    {
        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        pd.PrintQueue = queue;
       

        //var pCanvas = GetCanvas(SelectedPrintContent, SelectedLayout);
        //pd.PrintVisual(pCanvas, "printing Card");

    }

    public static void PrintTest(Format format,double offsetX, double offsetY,double printScale)
    {
        

        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        
        pd.PrintQueue = queue;
        //PageMediaSize pSize = new PageMediaSize(120.60, 53.98);
        //PageResolution pr = new PageResolution(1200, 1200);
        //pd.PrintTicket.PageMediaSize = pSize;
        //pd.PrintTicket.PageResolution = pr;
       
        var pCanvas = GetTestCanvas(format,offsetX, offsetY, printScale);
        pd.PrintVisual(pCanvas, "printing Card");


    }



    private static Canvas GetTestCanvas(Format format ,double offsetX, double offsetY, double scale)
    {

        var canvas = new Canvas();
        var width = Calc.MillimeterToPixel(format.Width, scale);
        var height = Calc.MillimeterToPixel(format.Height, scale);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width, height)));

        //Border
        Border border = new Border();
        border.BorderThickness = new Thickness(2);
        border.BorderBrush = Brushes.Black;
        border.CornerRadius = new CornerRadius(14);
        border.Width = width;
        border.Height = height;
        canvas.Children.Add(border);

        //Horizontal
        Line line1 = new Line();
        line1.Stroke = Brushes.Black;
        line1.StrokeThickness = 1;
        line1.X1 = 0;
        line1.Y1 = (height/2) + Calc.MillimeterToPixel(offsetY, scale);
        line1.X2 = width;
        line1.Y2 = (height/2) + Calc.MillimeterToPixel(offsetY, scale);
        canvas.Children.Add(line1);

        //vertical
        Line line2 = new Line();
        line2.Stroke = Brushes.Black;
        line2.StrokeThickness = 1;
        line2.X1 = (width/2) + Calc.MillimeterToPixel(offsetX, scale);
        line2.Y1 = 0;
        line2.X2 = (width/2) + Calc.MillimeterToPixel(offsetX, scale);
        line2.Y2 = height;
        canvas.Children.Add(line2);


        
        return canvas;

    }
    private static Canvas GetCanvas(Format format, double offsetX, double offsetY, double scale)
    {

        var canvas = new Canvas();
        var width = Calc.MillimeterToPixel(format.Width, scale);
        var height = Calc.MillimeterToPixel(format.Height, scale);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width, height)));

        //Border
        Border border = new Border();
        border.BorderThickness = new Thickness(2);
        border.BorderBrush = Brushes.Black;
        border.CornerRadius = new CornerRadius(14);
        border.Width = width;
        border.Height = height;
        canvas.Children.Add(border);

 

        return canvas;

    }
}
