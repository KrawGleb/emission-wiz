namespace EmissionWiz.Models.Templates;

public class SingleSourceReportModel
{
    public double A { get; set; }
    public double M { get; set; }
    public double FCoef { get; set; }
    public double H { get; set; }
    public double D { get; set; }
    public double W { get; set; }
    public double Eta { get; set; }
    public double U { get; set; }
    public double DeltaT { get; set; }

    public double VmResult { get; set; }
    public double VmIResult { get; set; }
    public double FResult { get; set; }
    public double FeResult { get; set; }
    public double V1Result { get; set; }
    public double CmResult { get; set; }
    public double CmuResult { get; set; }
    public double XmResult { get; set; }
    public double UmResult { get; set; }

    public double MICoef { get; set; }
    public double MCoef { get; set; }
    public double NCoef { get; set; }
    public double KCoef { get; set; }
    public double DCoef { get; set; }
    public double RCoef { get; set; }

    public double V1 => V1Result;
    public double Vm => VmResult;
    public double VmI => VmIResult;
    public double F => FResult;
    public double Fe => FeResult;
    public double Um => UmResult;

    public double WindRatio => U / UmResult;
    public bool WindRatio_G_1 => WindRatio > 1;

    public bool ColdEmission => (FResult >= 100 || (0 <= DeltaT && DeltaT <= 0.5)) && VmIResult > 0.5;
    public bool LowWind => (FResult < 100 && VmResult < 0.5) || (FResult >= 100 && VmIResult < 0.5);
    public bool HotEmission => !ColdEmission && !LowWind;

    public bool Vm_Less_05 => Vm < 0.5;
    public bool Vm_LoE_05 => Vm <= 0.5;
    public bool Vm_Less_2 => 0.5 <= Vm && Vm < 2;
    public bool Vm_LoE_2 => Vm <= 2;


    public bool VmI_Less_05 => VmI < 0.5;
    public bool VmI_LoE_05 => VmI <= 0.5;
    public bool VmI_LoE_2 => VmI <= 2;
    public bool VmI_Less_2 => 0.5 <= VmI && VmI < 2;

    public bool F_Less_100 => F < 100;
    public bool F_GoE_100 => F >= 100;
    public bool F_GoE_100_DeltaT_Between_0_05 => (0 <= DeltaT && DeltaT < 0.5) || F_GoE_100;
    public bool Fe_Less_f_Less100 => Fe < F && F < 100;
    public bool Fe_Less_100 => Fe < 100;
}
