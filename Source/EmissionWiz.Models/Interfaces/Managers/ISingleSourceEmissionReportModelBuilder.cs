﻿using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Templates;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISingleSourceEmissionReportModelBuilder : IBaseManager
{
    SingleSourceReportModel Build();

    ISingleSourceEmissionReportModelBuilder UseInputModel(SingleSourceCalculationData model);
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
    ISingleSourceEmissionReportModelBuilder SetPCoefValue(double p);
    ISingleSourceEmissionReportModelBuilder SetXmuValue(double xmu);
    ISingleSourceEmissionReportModelBuilder SetS1Value(double s1);
    ISingleSourceEmissionReportModelBuilder SetCValues(List<SingleSourceCCalculationModel> c);
    ISingleSourceEmissionReportModelBuilder SetS1HValue(double s1h);
    ISingleSourceEmissionReportModelBuilder SetS2Value(double s2);
    ISingleSourceEmissionReportModelBuilder SetTyValue(double ty);
    ISingleSourceEmissionReportModelBuilder SetCyValue(double cy);
}
