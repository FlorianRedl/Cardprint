using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardprint.Models;

namespace Cardprint.Utilities;


public static class Calc
{

    public static int MillimeterToPixel(double millimeter, double factor)
    {
        int dpi = 96;

        double inches = millimeter / 25.4;
        return (int)Math.Round(inches * dpi * factor);
    }


}
