using System.Xml.Serialization;

namespace Cardprint.Models;

public class TextField : IField
{
    [XmlElement("name")]
    public string Name { get; set; }
    [XmlElement("x")]
    public double XCord { get; set; }
    [XmlElement("y")]
    public double YCord { get; set; }
    [XmlElement("size")]
    public double Size { get; set; }
    [XmlElement("value")]
    public string? Text { get; set; }

    public TextField()
    {
        Name = string.Empty;
        XCord = 0;
        YCord = 0;
        Size = 1;
        Text = string.Empty;
    }

}
