
namespace Cardprint.Models;

public class FieldModel
{

    public string Name { get; set; }
    public double XCord { get; set; }
    public double YCord { get; set; }
    public double Size { get; set; }

    public FieldModel(string name, int xCord, int yCord, double size)
    {
        Name = name;
        XCord = xCord;
        YCord = yCord;
        Size = size;
    }
    public FieldModel()
    {

    }
}
