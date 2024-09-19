using System;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cardprint.Utilities;

static public class PrintHelper
{

    public static void Print(Canvas pCanvas,PrintQueue printQueue,int quantity)
    {
        try
        {
            PrintDialog pd = new PrintDialog();
            pd.PrintQueue = printQueue;
            for (int i = 0; i < quantity; i++)
            {
                pd.PrintVisual(pCanvas, $"printing Card [{i}]");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }

    
}