﻿using Cardprint.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cardprint.Views
{



    /// <summary>
    /// Interaktionslogik für MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private MainWindowViewModel mainWindowViewModel { get; }
        public MainWindowView()
        {

            DataContext = mainWindowViewModel = new MainWindowViewModel();
            mainWindowViewModel.OnSelectedLayoutChanges += UpdateGrid;
            InitializeComponent();
        }
        
        

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                mainWindowViewModel.SelectedIndex++;
            }
            else
            {
                mainWindowViewModel.SelectedIndex--;
            }
        }

        private void UpdateGrid(string[] headers)
        {
            Datagrid.Columns.Clear();
            for (int i = 0; i < headers.Length; i++)
            {
                DataGridTextColumn c = new DataGridTextColumn();

                var c2 = c as DataGridColumn;
                c.Header = headers[i];
                c.Binding = new Binding($"Field{i+1}");
                Datagrid.Columns.Add(c);               
                if (i == headers.Length-1) c.Width = new DataGridLength(10, DataGridLengthUnitType.Star); ;
            }  
        }

       
    }

    
}