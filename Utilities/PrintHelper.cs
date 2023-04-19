using Cardprint.Models;
using Cardprint.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Cardprint.Utilities;

static public class PrintHelper
{

    public static void Print(Canvas pCanvas)
    {
        PrintDialog pd = new PrintDialog();
        PrintQueue queue = new LocalPrintServer().GetPrintQueue(Settings.Default.SelectedPrinter);
        pd.PrintQueue = queue;
       
        //var pCanvas = GetCanvas(SelectedPrintContent, SelectedLayout);
        pd.PrintVisual(pCanvas, "printing Card");

    }

    
}
