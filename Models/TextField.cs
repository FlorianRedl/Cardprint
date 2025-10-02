using System.Xml.Serialization;

namespace Cardprint.Models;

public class TextField : Field
{

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
        Rotation = 0;
    }

}
