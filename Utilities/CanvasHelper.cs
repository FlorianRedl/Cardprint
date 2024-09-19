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
using System.Windows.Media.Imaging;
using System.IO;
namespace Cardprint.Views;

static class CanvasHelper
{
    static private SolidColorBrush GreyBrush { get { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A2A2A2")); }}


    public static Canvas GetViewBackground(Layout layout,double viewSize)
    {
        Canvas canvas = new();

        var width = MillimeterToPixel(layout.FormatSize.width, viewSize);
        var height = MillimeterToPixel(layout.FormatSize.height, viewSize);
        canvas.Width = width;
        canvas.Height = height;

        Border border = new Border();
        border.BorderThickness = new System.Windows.Thickness(1);
        border.BorderBrush = GreyBrush;
        border.CornerRadius = new System.Windows.CornerRadius(15);
        border.Width = width;
        border.Height = height;

        canvas.Children.Add(border);
        return canvas;

    }
    public static Canvas GetCanvas(Dictionary<string,string> fieldValues, Layout layout, double viewSize, bool fieldNamesDisplayed)
    {
        if (layout == null) return new Canvas();

        var canvas = new Canvas();
        var width = MillimeterToPixel(layout.FormatSize.width, viewSize);
        var height = MillimeterToPixel(layout.FormatSize.height, viewSize);
        canvas.Width = width;
        canvas.Height = height;
        canvas.Arrange(new Rect(new Size(width, height)));


        foreach (var field in layout.Fields)
        {
            var x = MillimeterToPixel(field.XCord, viewSize);
            var y = MillimeterToPixel(field.YCord, viewSize);

            if(field is TextField textField)
            {
                if(!fieldNamesDisplayed && !fieldValues.ContainsKey(textField.Name)) continue;

                Label label = new Label();
                if (fieldValues.ContainsKey(textField.Name))
                {
                    label.Foreground = new SolidColorBrush(Colors.Black);
                    label.Content = fieldValues[textField.Name];
                }
                else
                {
                    label.Foreground = new SolidColorBrush(Colors.LightGray);
                    label.Content = textField.Name;
                }

                label.FontSize = textField.Size;
                label.Padding = new Thickness(0);
                
                canvas.Children.Add(label);
                Canvas.SetLeft(label, x);
                Canvas.SetTop(label, y);
            }  

            if(field is ImageField imageField)
            {
                if (!File.Exists(imageField.Path))
                {
                    if (fieldNamesDisplayed)
                    {
                        Label label = new Label();
                        label.Foreground = new SolidColorBrush(Colors.Red);
                        label.Content = "path not found";
                        canvas.Children.Add(label);
                        Canvas.SetLeft(label, x);
                        Canvas.SetTop(label, y);
                    }
                    continue;
                }

                Image image = new Image();
                image.Source = new BitmapImage(new Uri(imageField.Path));
                if (imageField.Width != 0) image.Width = MillimeterToPixel(imageField.Width, viewSize);
                if (imageField.Height != 0) image.Height = MillimeterToPixel(imageField.Height, viewSize);
                image.Stretch = Stretch.Uniform;
                canvas.Children.Add(image);
                Canvas.SetZIndex(image, -1);
                Canvas.SetLeft(image, x);
                Canvas.SetTop(image, y);
            }
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
