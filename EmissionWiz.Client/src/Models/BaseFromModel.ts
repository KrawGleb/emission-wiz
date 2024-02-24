import { observable, action, computed } from 'mobx';

type IBaseFormModelInternal = {
    __validators?: [];
    __isValidForm?: boolean;
    __errorFields?: string[];
};

type IFormFields<TModel> = {
    [Property in keyof TModel]: unknown;
};

export class BaseFormModel {
    @observable accessor validated: boolean = false;

    @action
    errorFor<T extends BaseFormModel>(name: keyof T) {
        return this.validationErrorsName(name);
    }

    @action
    isValid<T extends BaseFormModel>(name: keyof T): boolean {
        return !this.validationErrorsName(name) || this.validationErrorsName(name).length === 0;
    }

    getValue<T extends BaseFormModel>(name: keyof T, index?: number): T[keyof T] {
        const intThis = this as unknown as IFormFields<T>;
        return index ? (intThis[name] as Array<any>)[index] : intThis[name] as T[keyof T];
    }

    @action
    setValue<T extends BaseFormModel>(name: keyof T, value: unknown, index?: number, subName?: string) {
        const intThis = this as unknown as IFormFields<T>;
        if (index != undefined && subName) {
            const arr = intThis[name] as Array<any>;

            intThis[name] = arr.map((x, i) => {
                if (i !== index) return x;

                const route = subName.split('.')
                const lastStep = route.pop();
                let field = x;
                route.forEach(element => {
                    field = field[element];
                });
                field[lastStep!] = value;
                return x;
            });
        }
        else {
            intThis[name] = value;
        }
    }

    @action
    pushValue<T extends BaseFormModel>(name: keyof T, value: unknown) {
        const inThis = this as unknown as IFormFields<T>;
        if (inThis[name] instanceof Array)
            (inThis[name] as Array<any>).push(value)
    }

    @action
    validationErrorsName<T extends BaseFormModel>(name: keyof T): string[] {
        const sName = name as string;
        const key = '__validateError_' + sName[0].toUpperCase() + sName.substr(1);
        const errors = (this as unknown as { [key: string]: string[] })[key] as string[];
        return errors;
    }

    @computed
    get isFormValid() {
        const intThis = this as IBaseFormModelInternal;
        return intThis.__validators ? intThis.__isValidForm : true;
    }

    @computed
    get invalidFields() {
        const intThis = this as IBaseFormModelInternal;
        return intThis.__errorFields || [];
    }

    @computed
    get formValidatedClass() {
        return this.validated ? 'validated' : 'not-validated';
    }

    @action validate(): boolean {
        this.validated = true;
        return !!this.isFormValid;
    }
}
