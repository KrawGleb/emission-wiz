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

    @action errorFor<T extends BaseFormModel>(name: keyof T) {
        return this.validationErrorsName(name);
    }

    @action isValid<T extends BaseFormModel>(name: keyof T): boolean {
        return !this.validationErrorsName(name).length;
    }

    getValue<T extends BaseFormModel>(name: keyof T): T[keyof T] {
        const intThis = this as unknown as IFormFields<T>;
        return intThis[name] as T[keyof T];
    }

    @action setValue<T extends BaseFormModel>(name: keyof T, value: unknown) {
        const intThis = this as unknown as IFormFields<T>;
        intThis[name] = value;
    }

    @action
    validationErrorsName<T extends BaseFormModel>(name: keyof T): string[] {
        const sName = name as string;
        const key = '__validateError_' + sName[0].toUpperCase() + sName.substr(1);
        const errors = (this as unknown as {[key: string]: string[]})[key] as string[];
        return errors ? errors : [];
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
