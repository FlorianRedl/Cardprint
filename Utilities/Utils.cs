using Cardprint.Properties;
using System;
using System.Collections.Generic;
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
        var server = new PrintServer();
        var localServer = new LocalPrintServer();
        var localPrintQueue = localServer.GetPrintQueue(printerName);
        if(localPrintQueue is not null)
        {
            return localPrintQueue;
        }

        var printQueues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
        return printQueues.FirstOrDefault(s => s.Name == printerName);
    }
}
