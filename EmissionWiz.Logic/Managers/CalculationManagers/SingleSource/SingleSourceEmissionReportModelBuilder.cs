using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Templates;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

public class SingleSourceEmissionReportModelBuilder : ISingleSourceEmissionReportModelBuilder
{
    private SingleSourceReportModel _model = new();

    public ISingleSourceEmissionReportModelBuilder UseInputModel(SingleSourceInputModel model)
    {
        _model.D = model.D;
        _model.DeltaT = model.DeltaT;
        _model.H = model.H;
        _model.W = model.W;
        _model.Eta = model.Eta;
        _model.FCoef = model.FCoef;
        _model.M = model.M;
        _model.A = model.A;
        _model.U = model.U;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder UseSourceProperties(EmissionSourceProperties sourceProperties)
    {
        _model.V1Result = sourceProperties.V1;
        _model.VmResult = sourceProperties.Vm;
        _model.VmIResult = sourceProperties.VmI;
        _model.FResult = sourceProperties.F;
        _model.FeResult = sourceProperties.Fe;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetMCoefValue(double m)
    {
        _model.MCoef = m;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetNCoefValue(double n)
    {
        _model.NCoef = n;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetKCoefValue(double k)
    {
        _model.KCoef = k;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetCmResultValue(double cm)
    {
        _model.CmResult = cm;
        
        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetMICoefValue(double mi)
    {
        _model.MICoef = mi;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetDCoefValue(double d) 
    {
        _model.DCoef = d;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetXmValue(double xm)
    {
        _model.XmResult = xm;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetUmValue(double vm)
    {
        _model.UmResult = vm;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetCmuValue(double cmu)
    {
        _model.CmuResult = cmu;

        return this;
    }

    public ISingleSourceEmissionReportModelBuilder SetRCoefValue(double r)
    {
        _model.RCoef = r;

        return this;
    }



    public SingleSourceReportModel Build() => _model; 
}
