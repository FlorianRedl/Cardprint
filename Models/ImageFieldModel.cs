using System;


namespace Cardprint.Models;

public class ImageFieldModel : IField
{
    public string Name { get; set; }
    public string Path { get; set; }
    public double XCord { get; set; }
    public double YCord { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }


    public ImageFieldModel(string name, double xCord, double yCord,double width,double heigth, string path)
    {
        Name = name;
        Path = path;
        XCord = xCord;
        YCord = yCord;
        Width = width;
        Height = heigth;


    }
}
