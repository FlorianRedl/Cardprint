using System;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cardprint.Utilities;

static public class PrintHelper
{

    public static void Print(Canvas pCanvas,string printerName,int quantity)
    {
        try
        {
            PrintDialog pd = new PrintDialog();
            PrintQueue queue = new LocalPrintServer().GetPrintQueue(printerName);
            pd.PrintQueue = queue;
            for (int i = 0; i < quantity; i++)
            {
                pd.PrintVisual(pCanvas, $"printing Card [{i}]");
            }
            

        }
        catch (Exception ex)
        {
            MessageBox.Show("incorrect layout!" + "\n \n" + $"{ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }

    
}