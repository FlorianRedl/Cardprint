using System;

namespace Cardprint.Models;

public interface IField
{
    public string Name { get; set; }
    public double XCord { get; set; }
    public double YCord { get; set; }
}
