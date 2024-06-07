using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Cardprint.Models;
using System.Collections.Generic;


namespace Cardprint.Models;

public partial class LayoutModel : ObservableObject
{

    public string LayoutName { get; set; }
    //public string? BackgroundImg { get; set; }
    public string? Format { get; set; }

    public (double width,double height) FormatSize { get { return GetFormatSize(); } }

    public List<IField> Fields { get; set; }


    //public bool IsValide(out string error)
    //{
    //    error= string.Empty;
    //    if (Format == null) { error = "missing Format"; return false; };
    //    if (Fields.Count == 0) { error = "no fields"; return false; };
    //    return true;
    //}

    private (double width, double height) GetFormatSize()
    {
        if (Format == null) return (0,0); 
        switch (Format)
        {
            case "ID-0":
                return (15, 82);
            case "ID-1":
                return (53.98, 85.60);
            case "ID-2":
                return (74, 105);
            case "ID-3":
                return (88, 125);
            default:
                return (53.98, 85.60);
        }
    }

    public LayoutModel(string layoutName,string? format, List<IField> fields)
    {
        LayoutName = layoutName;
        Fields = fields;
        Format = format;
    }


}
