export class BaseApiModelClass< T > {
    constructor(data?: T & {}) {
        if (data)
            for (let property in data)
                if (data.hasOwnProperty(property))
                    (< any >this)[property] = (< any >data)[property];
    }
}

export interface ISingleSourceEmissionCalculationResult {
    c: number;
    cy: number;
    cm: number;
    cmu: number;
    um: number;
    xm: number;
    xmu: number;
}
export class SingleSourceEmissionCalculationResult extends BaseApiModelClass< ISingleSourceEmissionCalculationResult > implements ISingleSourceEmissionCalculationResult {
    c: number;
    cy: number;
    cm: number;
    cmu: number;
    um: number;
    xm: number;
    xmu: number;
}

export interface ISingleSourceInputModel {
    a: number;
    m: number;
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
}
export class SingleSourceInputModel extends BaseApiModelClass< ISingleSourceInputModel > implements ISingleSourceInputModel {
    a: number;
    m: number;
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
}   