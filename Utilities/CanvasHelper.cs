using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardprint.Models;
using Cardprint.Utilities;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Cardprint.Views;

static class CanvasHelper
{
    static private SolidColorBrush GreyBrush { get { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A2A2A2")); }}


    public static Canvas GetViewBackground(LayoutModel layout,double viewSize)
    {
        Canvas canvas = new();

        var width = MillimeterToPixel(layout.FormatSize.height, viewSize);
        var height = MillimeterToPixel(layout.FormatSize.width, viewSize);
        canvas.Width = width;
        canvas.Height = height;

        Border border = new Border();
        border.BorderThickness = new System.Windows.Thickness(2);
        border.BorderBrush = GreyBrush;
        border.CornerRadius = new System.Windows.CornerRadius(15);
        border.Width = width;
        border.Height = height;

        canvas.Children.Add(border);
        return canvas;

    }
    static public Canvas GetViewCard(Dictionary<string,string> fieldValues, LayoutModel layout, double viewSize)
    {
        if (layout == null) return new Canvas();

        var canvas = new Canvas();
        var width = Calc.MillimeterToPixel(layout.FormatSize.height, viewSize);
        var height = Calc.MillimeterToPixel(layout.FormatSize.width, viewSize);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width, height)));

        int fieldIndex = 1;
        foreach (var field in layout.Fields)
        {
            Label label = new Label();

            if (fieldValues.ContainsKey(field.Name))
            {
                label.Foreground = new SolidColorBrush(Colors.Black);
                label.Content = fieldValues[field.Name];
            }
            else
            {
                label.Foreground = new SolidColorBrush(Colors.LightGray);
                label.Content = field.Name;
            }

            label.FontSize = field.Size;
            label.Padding = new Thickness(0);
            canvas.Children.Add(label);
            var x = Calc.MillimeterToPixel(field.XCord, viewSize);
            var y = Calc.MillimeterToPixel(field.YCord, viewSize);
            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            fieldIndex++;
        }

        return canvas;

    }

    
    public static int MillimeterToPixel(double millimeter, double factor)
    {
        int dpi = 96;

        double inches = millimeter / 25.4;
        return (int)Math.Round(inches * dpi * factor);
    }
}
