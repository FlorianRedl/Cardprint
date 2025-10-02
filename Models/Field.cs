using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cardprint.Models;

public class Field
{
    [XmlElement("name")]
    public string Name { get; set; }


    [XmlElement("x")]
    public double XCord { get; set; }


    [XmlElement("y")]
    public double YCord { get; set; }

    [XmlElement("rotation")]
    public double Rotation { get; set; }
}
