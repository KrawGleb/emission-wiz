import { Form, Input, InputProps, TooltipProps } from "antd";
import { BaseFormModel } from "../Models/BaseFromModel";
import React from "react";
import { observer } from "mobx-react";

export interface IFormInputProps<T extends BaseFormModel> extends Omit<InputProps, 'name'> {
    formModel: T;
    name: keyof T;
    invalid?: boolean;
    smallValidationError?: boolean;
    placeholder?: string;
    changeHandler?: Function;
    transformValueHandler?: (val: string) => string;
    style?: React.CSSProperties;
    className?: string;
    tooltip?: TooltipProps;
}


@observer
export class FormInput<T extends BaseFormModel> extends React.Component<IFormInputProps<T>, {}> {
    public render() {
        const { formModel, name, changeHandler, invalid, smallValidationError, transformValueHandler, style, className, tooltip, ...rest } = this.props;
        const fieldValue = formModel.getValue(name);
        const isInvalid: boolean | undefined = invalid || (formModel.validated ? !formModel.isValid(name) : formModel.validated);

        return (
            <Form>
                <Form.Item 
                    tooltip={tooltip}>
                    <Input
                        style={style}
                        className={className}
                        value={fieldValue as string}
                        autoComplete="off"
                        status={isInvalid ? 'error' : ''}
                        onChange={(ev) => {
                            const value = transformValueHandler ? transformValueHandler(ev.target.value) : ev.target.value;
                            formModel.setValue(name, value);
                            changeHandler?.(ev);
                        }}
                        {...rest} />
                </Form.Item>
                {formModel.invalidFields.includes(name as string) && (
                    <Form.ErrorList className={smallValidationError ? 'label-small' : ''} errors={formModel.errorFor(name).map((error: string) =>
                        <div key={(name as string) + error}>{error}</div>
                    )} />
                )}
            </Form>
        )
    }
}