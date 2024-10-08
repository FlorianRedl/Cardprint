﻿using System.Xml.Serialization;

namespace Cardprint.Models;

public class ImageField : IField
{
    [XmlElement("name")]
    public string Name { get; set; }
    [XmlElement("path")]
    public string Path { get; set; }
    [XmlElement("x")]
    public double XCord { get; set; }
    [XmlElement("y")]
    public double YCord { get; set; }
    [XmlElement("width")]
    public double Width { get; set; }
    [XmlElement("height")]
    public double Height { get; set; }

    public ImageField()
    {
        Name = string.Empty;
        Path = string.Empty;
        XCord = 0;
        YCord = 0;
        Width = 0;
        Height = 0;
    }
}
