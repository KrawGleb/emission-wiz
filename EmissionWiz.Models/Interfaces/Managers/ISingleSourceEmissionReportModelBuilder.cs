using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Templates;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISingleSourceEmissionReportModelBuilder
{
    ISingleSourceEmissionReportModelBuilder UseInputModel(SingleSourceInputModel model);
    ISingleSourceEmissionReportModelBuilder UseSourceProperties(EmissionSourceProperties sourceProperties);
    ISingleSourceEmissionReportModelBuilder SetMCoefValue(double m);
    ISingleSourceEmissionReportModelBuilder SetNCoefValue(double n);
    ISingleSourceEmissionReportModelBuilder SetMICoefValue(double mi);
    ISingleSourceEmissionReportModelBuilder SetKCoefValue(double k);
    ISingleSourceEmissionReportModelBuilder SetCmResultValue(double cm);
    ISingleSourceEmissionReportModelBuilder SetDCoefValue(double d);
    ISingleSourceEmissionReportModelBuilder SetXmValue(double xm);
    ISingleSourceEmissionReportModelBuilder SetUmValue(double vm);
    ISingleSourceEmissionReportModelBuilder SetCmuValue(double cmu);
    ISingleSourceEmissionReportModelBuilder SetRCoefValue(double r);

    SingleSourceReportModel Build();
}
