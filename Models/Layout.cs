using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Cardprint.Models;
using System.Collections.Generic;


namespace Cardprint.Models;

public partial class Layout : ObservableObject
{

    public string LayoutName { get; set; }
    //public string? BackgroundImg { get; set; }
    public string? Format { get; set; }

    public (double width,double height) FormatSize { get { return GetFormatSize(); } }

    public List<IField> Fields { get; set; }

    private (double width, double height) GetFormatSize()
    {
        if (Format == null) return (0,0); 
        switch (Format)
        {
            case "ID-0":
                return (25, 15);
            case "ID-1":
                return (85.60, 53.98);
            case "ID-2":
                return (105, 74);
            case "ID-3":
                return (125,88);
            default:
                return (85.60, 53.98);
        }
    }

    public Layout(string layoutName,string? format, List<IField> fields)
    {
        LayoutName = layoutName;
        Fields = fields;
        Format = format;
    }


}
