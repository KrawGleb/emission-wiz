import { computed } from "mobx";

export function isNumber(msg?: string) {
    return <any>function (target: any, { name }: { name: string }) {
        const validation = {
            fieldName: name,
            validateFunction: (obj: any) => {
                let isValid = !obj[name] || !isNaN(Number(obj[name]));
                let errorMEssage = `Field "${getDisplayName(obj, name)}" is not a number`;
                if (isValid && !Number.isInteger(Number(obj[name]))) {
                    isValid = false;
                    errorMEssage = `Field "${getDisplayName(obj, name)}" should be integer value.`;
                }
                return isValid ? undefined : msg || errorMEssage;
            }
        };

        addValidation(target, name, validation);
    };
}

function addValidation(target: any, name: any, validationRule: any) {
    const __validators = '__validators';
    const __validationErrors = '__validationErrors';
    const __isValidForm = '__isValidForm';
    const __validateError = camelCase('__validateError_', name);
    const __errorFields = '__errorFields';

    if (!target.hasOwnProperty(__validators)) {
        const prototypeValue = target[__validators];
        Object.defineProperty(target, __validators, {
            configurable: true,
            enumerable: false,
            value: prototypeValue?.slice(0) ?? []
        });
    }

    if (!target.hasOwnProperty(__validationErrors)) {
        const descriptor = {
            configurable: true,
            enumerable: false,
            get: function getter(this: any) {
                const errorList: any = [];
                const validators = this[__validators];
                validators.forEach((validator: any) => {
                    const error = validator.validateFunction(this);
                    if (error) {
                        errorList.push(error);
                    }
                });
                return errorList;
            }
        };
        defineComputedProperty(target, __validationErrors, descriptor);
    }

    if (!target.hasOwnProperty(__isValidForm)) {
        const descriptor = {
            configurable: true,
            enumerable: false,
            get: function getter(this: any) {
                let isValid = true;
                const validators = this[__validators];
                if (!validators.length) return isValid;
                validators.forEach((validator: any) => {
                    const error = validator.validateFunction(this);
                    if (error) {
                        isValid = false;
                    }
                });
                return isValid;
            }
        };

        defineComputedProperty(target, __isValidForm, descriptor);
    }

    if (!target.hasOwnProperty(__validateError)) {
        const descriptor = {
            configurable: true,
            enumerable: false,
            get: function getter(this: any) {
                const validators = this[__validators];
                const errorList: any = [];
                validators.forEach((validator: any) => {
                    if (validator.fieldName === name) {
                        const error = validator.validateFunction(this);
                        if (error) {
                            errorList.push(error);
                        }
                    }
                });
                return errorList;
            }
        };
        defineComputedProperty(target, __validateError, descriptor);
    }

    if (!target.hasOwnProperty(__errorFields)) {
        const descriptor = {
            configurable: true,
            enumerable: false,
            get: function getter(this: any) {
                const validators = this[__validators];
                const errorNames: any = [];
                validators.forEach((validator: any) => {
                    const error = validator.validateFunction(this);
                    if (error) {
                        errorNames.push(validator.fieldName);
                    }
                });
                return errorNames;
            }
        };
        defineComputedProperty(target, __errorFields, descriptor);
    }

    target[__validators].push(validationRule);
}

function defineComputedProperty(target: any, name: string, descriptor: PropertyDescriptor & ThisType<any>) {
    Object.defineProperty(target, name, descriptor);
    computed(target, name);
}

function camelCase(prefix: string, others: string) {
    return prefix + others[0]?.toUpperCase() + others?.substring?.(1);
}

function getDisplayName(target: any, name: string) {
    const __displayName = camelCase('____displayName_', name);
    return target[__displayName] ? target[__displayName] : camelCase('', name);
}

export function setDisplayName(target: any, name: string, value: string) {
    const __displayName = camelCase('____displayName_', name);
    target[__displayName] = value;
}
