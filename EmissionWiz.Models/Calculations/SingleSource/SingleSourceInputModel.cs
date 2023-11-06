namespace EmissionWiz.Models.Calculations.SingleSource;

public class SingleSourceInputModel
{
    /// <summary>
    /// Коэффициент, зависящий от температурной стратификации атмосферы, определяющий условия горизонтального и вертикального рассеивания ЗВ в атмосферном воздухе 
    /// </summary>
    public double A { get; set; }

    /// <summary>
    /// Масса ЗВ, выбрасываемого в атмосферный воздух в единицу времени (мощность выброса), г/с
    /// </summary>
    public double M { get; set; }

    /// <summary>
    /// Безразмерный коэффициент, учитывающий скорость оседания ЗВ (газообразных и аэрозолей, включая твердые частицы) в атмосферном воздухе
    /// </summary>
    public double FCoef { get; set; }

    /// <summary>
    /// Высота источника выброса, м
    /// </summary>
    public double H { get; set; }

    /// <summary>
    /// Диаметр устья источника выброса, м
    /// </summary>
    public double D { get; set; }

    /// <summary>
    /// Средняя скорость выхода ГВС из устья источника выброса, м/с
    /// </summary>
    public double W { get; set; }

    /// <summary>
    /// Безразмерный коэффициент, учитывающий влияние рельефа местности
    /// </summary>
    public double Eta { get; set; }

    /// <summary>
    /// Тв - Температурой атмосферного воздуха, °C
    /// </summary>
    public double AirTemperature { get; set; }

    /// <summary>
    /// Tг - Температура выбрасываемой ГВС, °C
    /// </summary>
    public double EmissionTemperature { get; set; }

    /// <summary>
    /// Скорость ветра
    /// </summary>
    public double U { get; set; }

    public double DeltaT => EmissionTemperature - AirTemperature;
}
