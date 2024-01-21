export class BaseApiModelClass< T > {
    constructor(data?: T & {}) {
        if (data)
            for (let property in data)
                if (data.hasOwnProperty(property))
                    (< any >this)[property] = (< any >data)[property];
    }
}

export interface ISingleSourceEmissionCalculationResult {
    name: string;
    reportId: string;

    c: number;
    cy: number;
    cm: number;
    cmu: number;
    um: number;
    xm: number;
    xmu: number;
}
export class SingleSourceEmissionCalculationResult extends BaseApiModelClass< ISingleSourceEmissionCalculationResult > implements ISingleSourceEmissionCalculationResult {
    name: string;
    reportId: string;

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
export class SingleSourceEmissionSubstance extends BaseApiModelClass< ISingleSourceEmissionSubstance > implements ISingleSourceEmissionSubstance {
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
}
export class SingleSourceInputModel extends BaseApiModelClass< ISingleSourceInputModel > implements ISingleSourceInputModel {
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
    substances?: SingleSourceEmissionSubstance[];
}   

export interface ISingleSourceEmissionInputModel {
    name?: string;
    m?: number;
}
export class SingleSourceEmissionInputModel extends BaseApiModelClass <ISingleSourceEmissionInputModel> implements SingleSourceEmissionInputModel {
    name?: string;
    m?: number;
}