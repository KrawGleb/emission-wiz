/* eslint-disable no-prototype-builtins */
/* eslint-disable @typescript-eslint/no-explicit-any */
import { computed } from "mobx";

export class collections {
    public static numbers(path?: string, msg?: string) {
        return <any>function (_: any, context: ClassAccessorDecoratorContext) {
            const name = String(context.name);

            const validation = {
                fieldName: name,
                validateFunction: (obj: any) => {
                    const pathParts = path?.split('.');
                    const field = obj[name] as Array<any>;
                    let isValid = true;
                    
                    field?.forEach(element => {
                        if (pathParts?.length) {
                            let subField = element;
                            pathParts.forEach(part => subField = subField?.[part])

                            const asNumber = Number(subField);
                            isValid = isValid && (!subField || (!isNaN(asNumber) && isFinite(asNumber)));
                        }
                    })

                    const errorMessage = `${getDisplayName(obj, name)} долнжо содержать только числа`;

                    return isValid ? undefined : msg || errorMessage;
                }
            }

            addValidation(context, name, validation);
        }
    }

    public static notEmpty(msg?: string) {
        return <any>function (_: any, context: ClassAccessorDecoratorContext) {
            const name = String(context.name);

            const validation = {
                fieldName: name,
                validateFunction: (obj: any) => {
                    const field = obj[name] as Array<any>;
                    const isValid = field && field.length > 0; 
                    
                    const errorMessage = `${getDisplayName(obj, name)} должно содержать значение`;

                    return isValid ? undefined : msg || errorMessage;
                }
            }

            addValidation(context, name, validation);
        }
    }
}

export function displayName(displayName: string) {
    return <any>function (target: any, name: string) {
        const __displayName = camelCase('____displayName_', name);
        target[__displayName] = displayName;
    };
}

export function isRequired(msg?: string) {
    return <any>function (_: any, context: ClassAccessorDecoratorContext) {
        const name = String(context.name);

        const validation = {
            fieldName: name,
            validateFunction: (obj: any) => {
                return obj[name] ? null : msg || `Поле "${getDisplayName(obj, name)}" не может быть пустым`;
            }
        };

        addValidation(context, name, validation);
    };
}

export function isNumber(msg?: string) {
    return <any>function (_: any, context: ClassAccessorDecoratorContext) {
        const name = String(context.name);

        const validation = {
            fieldName: name,
            validateFunction: (obj: any) => {
                const isValid = !obj[name] || !isNaN(Number(obj[name]));
                const errorMessage = `Поле "${getDisplayName(obj, name)}" должно быть числом`;
                return isValid ? undefined : msg || errorMessage;
            }
        };

        addValidation(context, name, validation);
    };
}

function addValidation(context: ClassAccessorDecoratorContext, name: any, validationRule: any) {
    const __validators = '__validators';
    const __validationErrors = '__validationErrors';
    const __isValidForm = '__isValidForm';
    const __validateError = camelCase('__validateError_', name);
    const __errorFields = '__errorFields';

    context.addInitializer(function () {
        const targetForm = this as any;
        if (!targetForm.hasOwnProperty(__validators)) {
            const prototypeValue = targetForm[__validators];
            Object.defineProperty(targetForm, __validators, {
                configurable: true,
                enumerable: false,
                value: prototypeValue?.slice(0) ?? []
            });
        }

        if (!targetForm.hasOwnProperty(__validationErrors)) {
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
            defineComputedProperty(targetForm, __validationErrors, descriptor);
        }

        if (!targetForm.hasOwnProperty(__isValidForm)) {
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

            defineComputedProperty(targetForm, __isValidForm, descriptor);
        }

        if (!targetForm.hasOwnProperty(__validateError)) {
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
            defineComputedProperty(targetForm, __validateError, descriptor);
        }

        if (!targetForm.hasOwnProperty(__errorFields)) {
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
            defineComputedProperty(targetForm, __errorFields, descriptor);
        }

        targetForm[__validators].push(validationRule);
    })
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
