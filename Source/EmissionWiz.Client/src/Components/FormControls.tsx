import React from 'react';
import { Checkbox, Form, Input, InputProps, Tooltip, TooltipProps } from 'antd';
import { observer } from 'mobx-react';
import { observable } from 'mobx';

import { BaseFormModel } from '../Models/BaseFromModel';
import { CheckboxChangeEvent } from 'antd/es/checkbox';

export type FormInputType<T> = Omit<InputProps, 'name'> &
    (
        | {
              name?: string;
              formModel?: undefined;
          }
        | {
              name: keyof T;
              formModel?: T;
          }
    );

export interface IFormInputProps<T extends BaseFormModel> extends Omit<InputProps, 'name'> {
    formModel: T;
    name: keyof T;
    index?: number;
    subName?: string;
    invalid?: boolean;
    smallValidationError?: boolean;
    placeholder?: string;
    changeHandler?: (value: React.ChangeEvent<HTMLInputElement>) => void;
    transformValueHandler?: (val: string) => string;
    style?: React.CSSProperties;
    className?: string;
    tooltip?: TooltipProps;
    label?: string;
    addonAfter?: string;
    hideValidationErrors?: boolean;
}

@observer
export class FormInput<T extends BaseFormModel> extends React.Component<IFormInputProps<T>> {
    @observable
    private accessor _wasChanged: boolean = false;

    public render() {
        const { formModel, name, addonAfter, hideValidationErrors, subName, index, changeHandler, invalid, smallValidationError, transformValueHandler, style, className, tooltip, label, ...rest } = this.props;
        const fieldValue = formModel.getValue(name);
        const isInvalid: boolean | undefined = this._wasChanged && (invalid || !formModel.isValid(name));

        return (
            <Form style={style}>
                <Form.Item label={label}>
                    <Tooltip {...tooltip}>
                        <Input
                            style={style}
                            className={className}
                            value={fieldValue as string}
                            autoComplete="off"
                            status={isInvalid ? 'error' : ''}
                            addonAfter={addonAfter}
                            onChange={(ev) => {
                                this._wasChanged = true;
                                const value = transformValueHandler ? transformValueHandler(ev.target.value) : ev.target.value;
                                formModel.setValue(name, value, index, subName);
                                changeHandler?.(ev);
                            }}
                            {...rest}
                        />
                    </Tooltip>
                </Form.Item>
                {this._wasChanged && !hideValidationErrors && formModel.invalidFields.includes(name as string) && (
                    <Form.ErrorList
                        className={smallValidationError ? 'label-small' : ''}
                        errors={formModel.errorFor(name).map((error: string) => (
                            <div key={(name as string) + error} className="text-danger">
                                {error}
                            </div>
                        ))}
                    />
                )}
            </Form>
        );
    }
}

export type IFormCheckboxProps<T extends BaseFormModel> = FormInputType<T> & {
    label?: string;
    checked: boolean;
    invalid?: boolean;
    disabled?: boolean;
    // eslint-disable-next-line @typescript-eslint/ban-types
    changeHandler?: Function | void;
    bsSize?: 'lg' | 'sm';
    className?: string;
    topAligned?: boolean;
    smallValidationError?: boolean;
    customLabel?: () => JSX.Element;
};

@observer
export class FormCheckbox<T extends BaseFormModel> extends React.Component<IFormCheckboxProps<T>> {
    public render() {
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { label, name, formModel, checked, disabled, changeHandler, bsSize, className, topAligned, title, customLabel, onChange, ...rest } = this.props;
        let isChecked: boolean = checked;
        const classList: string[] = ['checkbox'];
        if (formModel && name) {
            const value = Boolean(formModel.getValue(name as keyof T));
            isChecked = value === true;
        }
        if (bsSize) classList.push(bsSize);
        if (className) classList.push(className);
        if (topAligned) classList.push('top');

        return (
            <Form>
                <Form.Item label={title}>
                    <Checkbox
                        checked={isChecked}
                        disabled={disabled}
                        onChange={(event: CheckboxChangeEvent) => {
                            if (formModel) {
                                formModel.setValue(name as keyof T, event.target.checked);
                            }
                            if (changeHandler) changeHandler();
                        }}
                        name={name as string}
                        {...rest}
                    />
                    {customLabel ? customLabel() : <span>{label}</span>}
                </Form.Item>
                {formModel && formModel.validated && formModel.invalidFields.includes(name as string) && (
                    <Form.ErrorList
                        errors={formModel.errorFor(name as keyof T).map((error: string) => (
                            <div key={(name as string) + error}>{error}</div>
                        ))}
                    />
                )}
            </Form>
        );
    }
}
