using Cardprint.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Cardprint.Models;

[XmlRoot("layout")]
public class Layout
{
    [XmlIgnore]
    public string Name { get; set; } = string.Empty;
    [XmlElement("format")]
    public string Format { get; set; } = string.Empty;
    [XmlElement("text")]
    public List<TextField> TextFields { get; set; } = new();
    [XmlElement("image")]
    public List<ImageField> ImageFields { get; set; } = new();

    [XmlIgnore]
    public List<IField> Fields { get { return TextFields.Concat<IField>(ImageFields).ToList(); } } 
    [XmlIgnore]
    public (double width, double height) FormatSize { get { return Utils.GetFormatSize(Format); } }

}


