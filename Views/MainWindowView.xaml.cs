﻿using Cardprint.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Cardprint.Models;
using System.Collections.Generic;
using System.Linq;

namespace Cardprint.Views;

public partial class MainWindowView : Window
{
    private MainWindowViewModel mainWindowViewModel { get; }
    public MainWindowView()
    {

        DataContext = mainWindowViewModel = new MainWindowViewModel();
        mainWindowViewModel.OnSelectedLayoutChanges += UpdateGrid;
        InitializeComponent();
    }
    

    private void UpdateGrid(string[]? headers)
    {

        DatagridPrintContent.Columns.Clear();
        if(headers == null || headers.Length == 0 ) return ;
        for (int i = 0; i < headers.Length; i++)
        {
            DataGridTextColumn c = new DataGridTextColumn();
            var c2 = c as DataGridColumn;
            c.Header = headers[i];
            c.Binding = new Binding($"Field{i+1}");
            DatagridPrintContent.Columns.Add(c);               
            if (i == headers.Length-1) c.Width = new DataGridLength(10, DataGridLengthUnitType.Star); ;
        }
        DataGridTextColumn countColumn = new DataGridTextColumn();
        countColumn.Header = "Quantity";
        countColumn.Binding = new Binding("Quantity");
        DatagridPrintContent.Columns.Add(countColumn);
    }

    private void Datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var items = DatagridPrintContent.SelectedItems.Cast<PrintContent>().ToList();
       // mainWindowViewModel.SelectedPrintContents = items;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        mainWindowViewModel.StartUp();
    }
}