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
using System.Windows.Shapes;

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
    public static Canvas GetViewCard(Dictionary<string,string> fieldValues, LayoutModel layout, double viewSize, bool emptyFieldsVisible)
    {
        if (layout == null) return new Canvas();

        var canvas = new Canvas();
        var width = MillimeterToPixel(layout.FormatSize.height, viewSize);
        var height = MillimeterToPixel(layout.FormatSize.width, viewSize);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width, height)));

        int fieldIndex = 1;
        foreach (var field in layout.Fields)
        {
            if (!emptyFieldsVisible && !fieldValues.ContainsKey(field.Name)) continue;

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
            var x = MillimeterToPixel(field.XCord, viewSize);
            var y = MillimeterToPixel(field.YCord, viewSize);
            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            fieldIndex++;
        }

        return canvas;

    }
    public static Canvas GetTestPrintCanvas(Format format, double offsetX, double offsetY, double scale)
    {

        var canvas = new Canvas();
        var width = MillimeterToPixel(format.Width, scale);
        var height = MillimeterToPixel(format.Height, scale);
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
        line1.Y1 = (height / 2) + MillimeterToPixel(offsetY, scale);
        line1.X2 = width;
        line1.Y2 = (height / 2) + MillimeterToPixel(offsetY, scale);
        canvas.Children.Add(line1);

        //vertical
        Line line2 = new Line();
        line2.Stroke = Brushes.Black;
        line2.StrokeThickness = 1;
        line2.X1 = (width / 2) + MillimeterToPixel(offsetX, scale);
        line2.Y1 = 0;
        line2.X2 = (width / 2) + MillimeterToPixel(offsetX, scale);
        line2.Y2 = height;
        canvas.Children.Add(line2);



        return canvas;

    }

    public static int MillimeterToPixel(double millimeter, double factor)
    {
        int dpi = 96;

        double inches = millimeter / 25.4;
        return (int)Math.Round(inches * dpi * factor);
    }
}
