import { WindDirection } from "../Components/WindRose";

export class BaseApiModelClass<T> {
  constructor(data?: T) {
    if (data)
      for (const property in data)
        if (Object.prototype.hasOwnProperty.call(data, property))
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          (<any>this)[property] = (<any>data)[property];
  }
}

export interface ISingleSourceEmissionCalculationResult {
  name: string;
  reportId: string;
  applicationIds: string[];

  c: number;
  cy: number;
  cm: number;
  cmu: number;
  um: number;
  xm: number;
  xmu: number;
}
export class SingleSourceEmissionCalculationResult
  extends BaseApiModelClass<ISingleSourceEmissionCalculationResult>
  implements ISingleSourceEmissionCalculationResult
{
  name: string;
  reportId: string;
  applicationIds: string[];

  c: number;
  cy: number;
  cm: number;
  cmu: number;
  um: number;
  xm: number;
  xmu: number;
}

export interface ISingleSourceEmissionSubstance {
  name: string;
  m: number;
}
export class SingleSourceEmissionSubstance
  extends BaseApiModelClass<ISingleSourceEmissionSubstance>
  implements ISingleSourceEmissionSubstance
{
  name: string;
  m: number;
}

export interface ISingleSourceInputModel {
  a: number;
  fCoef: number;
  h: number;
  d: number;
  w: number;
  eta: number;
  airTemperature: number;
  emissionTemperature: number;
  u?: number;
  x?: number;
  y?: number;
  lat?: number;
  lon?: number;
  substances?: ISingleSourceEmissionSubstance[];
  windRose: WindDirection[]
}
export class SingleSourceInputModel
  extends BaseApiModelClass<ISingleSourceInputModel>
  implements ISingleSourceInputModel
{
  a: number;
  fCoef: number;
  h: number;
  d: number;
  w: number;
  eta: number;
  airTemperature: number;
  emissionTemperature: number;
  u?: number;
  x?: number;
  y?: number;
  lat?: number;
  lon?: number;
  b?: number;
  l?: number;
  substances?: SingleSourceEmissionSubstance[];
  windRose: WindDirection[]
}

export interface ISubstanceDto {
  code: number;
  name: string;
  chemicalFormula?: string;
  dangerClass: number;
  singleMaximumAllowableConcentration?: number;
  dailyAverageMaximumAllowableConcentration?: number;
  annualAverageMaximumAllowableConcentration?: number;
}

export class SubstanceDto
  extends BaseApiModelClass<ISubstanceDto>
  implements ISubstanceDto
{
  code: number;
  name: string;
  chemicalFormula?: string;
  dangerClass: number;
  singleMaximumAllowableConcentration?: number;
  dailyAverageMaximumAllowableConcentration?: number;
  annualAverageMaximumAllowableConcentration?: number;
}

export interface ISplineData {
  xs: number[];
  ys: number[];
  count: number;
}

export class SplineData extends BaseApiModelClass<ISplineData> implements ISplineData {
  xs: number[];
  ys: number[];
  count: number;
}