import { Form, Input, InputProps, Tooltip, TooltipProps } from "antd";
import { BaseFormModel } from "../Models/BaseFromModel";
import React from "react";
import { observer } from "mobx-react";
import { observable } from "mobx";

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
}


@observer
export class FormInput<T extends BaseFormModel> extends React.Component<IFormInputProps<T>> {
    @observable
    private accessor _wasChanged: boolean = false;

    public render() {
        const { formModel, name, subName, index, changeHandler, invalid, smallValidationError, transformValueHandler, style, className, tooltip, ...rest } = this.props;
        const fieldValue = formModel.getValue(name);
        const isInvalid: boolean | undefined = this._wasChanged && (invalid || !formModel.isValid(name));

        return (
            <Form style={style}>
                <Form.Item >
                    <Tooltip {...tooltip}>
                        <Input
                            style={style}
                            className={className}
                            value={fieldValue as string}
                            autoComplete="off"
                            status={isInvalid ? 'error' : ''}
                            onChange={(ev) => {
                                this._wasChanged = true;
                                const value = transformValueHandler ? transformValueHandler(ev.target.value) : ev.target.value;
                                formModel.setValue(name, value, index, subName);
                                changeHandler?.(ev);
                            }}
                            {...rest} />
                    </Tooltip>
                </Form.Item>
                {this._wasChanged && formModel.invalidFields.includes(name as string) && (
                    <Form.ErrorList className={smallValidationError ? 'label-small' : ''} errors={formModel.errorFor(name).map((error: string) =>
                        <div key={(name as string) + error} className="text-danger">{error}</div>
                    )} />
                )}
            </Form>
        )
    }
}