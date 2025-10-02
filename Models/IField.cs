using System;

namespace Cardprint.Models;

public interface IField
{
    public double XCord { get; set; }
    public double YCord { get; set; }
    public double Rotation { get; set; }
}
