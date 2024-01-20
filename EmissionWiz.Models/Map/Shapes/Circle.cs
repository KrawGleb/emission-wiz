namespace EmissionWiz.Models.Map.Shapes;

public class Circle : Shape
{
    public Coordinates Center { get; set; } = null!;
    public double Radius { get; set; }

    // ?
    // public string Color { get; set; }
}
