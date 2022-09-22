using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardprint.Properties;

namespace Cardprint.ViewModels;

[ObservableObject]
internal partial class SettingsViewModel
{

    [ObservableProperty]
    public int resolution;

    public SettingsViewModel()
    {
        Resolution = Settings.Default.Resolution;


    }

    

    [RelayCommand]
    private void Save()
    {
        Settings.Default.Resolution = resolution;
        Settings.Default.Save();
    }

}
