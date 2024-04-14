interface ICallbackFunction extends Function {
    selectTimeOut?: number;
    amountOfCall?: number;
}

export default class Extensions {
    static executeTimeout(callBack: Function, timeout: number, scope?: unknown, ...args: unknown[]) {
        const extendedFunction = <ICallbackFunction>callBack;
        if (extendedFunction.selectTimeOut) {
            clearTimeout(extendedFunction.selectTimeOut);
        }
        extendedFunction.selectTimeOut = window.setTimeout(() => {
            callBack.apply(scope, args);
        }, timeout);
    }
}