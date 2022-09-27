using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Cardprint.Models;
using System.Collections.Generic;


namespace Cardprint.Models;

public partial class LayoutModel : ObservableObject
{

    public int LayoutId { get; set; }
    public string LayoutName { get; set; }
    public string? BackgroundImg { get; set; }
    public string? Format { get; set; }

    public (double width,double height) FormatSize { get { return GetFormatSize(); } }

    public List<FieldModel> Fields { get; set; }


    public bool IsError()
    {
        if (Format == null) return true;
        return false;
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

    public LayoutModel(int layoutId, string layoutName,string? backgroundImg,string? format, List<FieldModel> fields)
    {
        LayoutId = layoutId;
        LayoutName = layoutName;
        Fields = fields;
        BackgroundImg = backgroundImg;
        Format = format;
    }



    public LayoutModel()
    {

    }


}
