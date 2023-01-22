using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardprint.Models;

public class Format
{
    public string Name { get; set; }
    public string NameLong { get { return  $"{Name} [{Width} x {Height}]"; } }
    public double Width { get; set; }
    public double Height { get; set; }

    
    public Format(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}
