using Cardprint.Properties;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Cardprint.Utilities;

internal static class Utils
{
    public static (double width, double height) GetFormatSize(string format)
    {
        if (string.IsNullOrEmpty(format)) return (0, 0);
        switch (format)
        {
            case "ID-0":
                return (25, 15);
            case "ID-1":
                return (85.60, 53.98);
            case "ID-2":
                return (105, 74);
            case "ID-3":
                return (125, 88);
            default:
                return (85.60, 53.98);
        }
    }

    public static PrintQueue? GetPrintQueueFromName(string printerName)
    {

        try
        {
            var installedPrinters = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            if(installedPrinters.Contains(printerName))
            {
                var localServer = new LocalPrintServer();
                var localPrintQueue = localServer.GetPrintQueue(printerName);
                return localPrintQueue;
            }

            var server = new PrintServer();
            var printQueues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            return printQueues.FirstOrDefault(s => s.Name == printerName);
        }
        catch (Exception)
        {

            return null;
        }
    }
}
