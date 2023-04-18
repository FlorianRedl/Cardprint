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

    public List<FieldModel> Fields { get; set; }


    public bool IsValide(out string error)
    {
        error= string.Empty;
        if (Format == null) { error = "missing Format"; return false; };
        if (Fields.Count == 0) { error = "no fields"; return false; };
        return true;
    }

    private (double width, double height) GetFormatSize()
    {
        switch (Format)
        {
            case "ID-1":
                return (53.98, 85.60);
            default:
                return (5, 5);
        }
    }

    public LayoutModel(string layoutName,string? format, List<FieldModel> fields)
    {
        LayoutName = layoutName;
        Fields = fields;
        Format = format;
    }


}
