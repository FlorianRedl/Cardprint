using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Cardprint.Models;
using System.Collections.Generic;

namespace Cardprint.Models;

public partial class LayoutModel : ObservableObject
{
    [ObservableProperty]
    public int layoutId;
    [ObservableProperty]
    public string layoutName;
    public string? BackgroundImg { get; set; }

    public List<FieldModel> Fields { get; set; }


    public LayoutModel(int layoutId, string layoutName,string? backgroundImg, List<FieldModel> fields)
    {
        LayoutId = layoutId;
        LayoutName = layoutName;
        Fields = fields;
        BackgroundImg = backgroundImg;
    }

    public LayoutModel()
    {

    }
}
