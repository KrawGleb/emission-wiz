namespace EmissionWiz.Models.Calculations.SingleSource;

public class EmissionSourceProperties
{
    private double v1;

    public double V1
    {
        get { return V1e != null ? V1e.Value : v1; }
        set { 
            v1 = value;
        }
    }

    public double V1Source { get; set; }
    public double? V1e { get; set; }
    public double Vm { get; set; }
    public double VmI { get; set; }
    public double F { get; set; }
    public double Fe { get; set; }

    public double RCoef { get; set; }
    public double PCoef { get; set; }
}
