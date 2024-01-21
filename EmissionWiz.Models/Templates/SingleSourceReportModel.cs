namespace EmissionWiz.Models.Templates;

public class SingleSourceReportModel : ReportModelBase
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
    public double X { get; set; }
    public double Y { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string? EmissionName { get; set; }

    public double VmResult { get; set; }
    public double VmIResult { get; set; }
    public double FResult { get; set; }
    public double FeResult { get; set; }
    public double V1Result { get; set; }
    public double CmResult { get; set; }
    public double CmuResult { get; set; }
    public double XmResult { get; set; }
    public double XmuResult { get; set; }
    public double UmResult { get; set; }
    public double CResult { get; set; }
    public double CyResult { get; set; }

    public double MICoef { get; set; }
    public double MCoef { get; set; }
    public double NCoef { get; set; }
    public double KCoef { get; set; }
    public double DCoef { get; set; }
    public double RCoef { get; set; }
    public double PCoef { get; set; }
    public double S1Coef { get; set; }
    public double S1HCoef { get; set; }
    public double S2Coef { get; set; }

    public double Ty { get; set; }

    public double V1 => V1Result;
    public double Vm => VmResult;
    public double VmI => VmIResult;
    public double F => FResult;
    public double Fe => FeResult;
    public double Um => UmResult;
    public double Xm => XmResult;

    public double WindRatio => U / UmResult;
    public bool WindRatio_LoE_025 => WindRatio <= 0.25;
    public bool WindRatio_LoE_1 => !WindRatio_LoE_025 && WindRatio <= 1;
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

    public double DistanceRatio => X / Xm;
    public bool S1_Case1 => DistanceRatio <= 1;
    public bool S1_Case2 => 1 < DistanceRatio && DistanceRatio <= 8;
    public bool S1_Case3 => 8 < DistanceRatio && DistanceRatio <= 100 && FCoef <= 1.5;
    public bool S1_Case4 => 8 < DistanceRatio && DistanceRatio <= 100 && FCoef > 1.5;
    public bool S1_Case5 => DistanceRatio > 100 && FCoef <= 1.5;
    public bool S1_Case6 => DistanceRatio > 100 && FCoef > 1.5;
    public bool S1_Case7 => DistanceRatio < 1 && H < 10;

    public bool Ty_Case1 => U <= 5;
    public bool Ty_Case2 => U > 5;
}
