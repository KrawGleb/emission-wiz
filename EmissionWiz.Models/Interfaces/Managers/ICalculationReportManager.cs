﻿using EmissionWiz.Models.Reports.Areas;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ICalculationReportManager
{
    ICalculationReportManager SetTitle(string title);
    void Generate(Stream destination);
    void AddBlock(BaseBlock block);
    void Reset();
}
