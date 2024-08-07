﻿namespace Cardprint.Models;

public class TextField : IField
{
    public string Name { get; set; }
    public double XCord { get; set; }
    public double YCord { get; set; }
    public double Size { get; set; }
    public string? Text { get; set; }

    public TextField(string name, double xCord, double yCord, double size, string? text)
    {
        Name = name;
        XCord = xCord;
        YCord = yCord;
        Size = size;
        Text = text;
    }
    
}
