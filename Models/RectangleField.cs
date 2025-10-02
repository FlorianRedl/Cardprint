using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cardprint.Models;

public class RectangleField : Field
{
    [XmlElement("width")]
    public double Width { get; set; }

    [XmlElement("height")]
    public double Height { get; set; }

    [XmlElement("color")]
    public string Color { get; set; } = string.Empty;
}
