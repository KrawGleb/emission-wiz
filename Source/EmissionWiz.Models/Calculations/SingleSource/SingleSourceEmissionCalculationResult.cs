﻿namespace EmissionWiz.Models.Calculations.SingleSource;

public class SingleSourceEmissionCalculationResult
{
    public string? Name { get; set; }
    public List<FileContent> Files { get; set; } = new();

    public double C { get; set; }
    public double Cm { get; set; }
    public double Cy { get; set; }
    public double Cmu { get; set; }
    public double Um { get; set; }
    public double Xm { get; set; }
    public double Xmu { get;set; }

    public Dictionary<string, List<string>> ValidationErrors { get; set; } = new();
}
