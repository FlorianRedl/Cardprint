namespace Cardprint.Models;

public class FieldModel
{
    public string Name { get; set; }
    public double XCord { get; set; }
    public double YCord { get; set; }
    public double Size { get; set; }
    public string Value { get; set; }

    public FieldModel(string name, int xCord, int yCord, double size, string value)
    {
        Name = name;
        XCord = xCord;
        YCord = yCord;
        Size = size;
        Value = value;
    }
    public FieldModel()
    {

    }
}
