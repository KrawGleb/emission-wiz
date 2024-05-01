import React from 'react';
import { Col, Space } from 'antd';
import { observable } from 'mobx';
import { observer } from 'mobx-react';

import { IModalDialogContent, ModalButtonType, ModalDialogOptions, ModalWindow } from '../Modal/Modal';
import { BaseFormModel } from '../../Models/BaseFromModel';
import { FormCheckbox, FormInput } from '../FormControls';
import { SingleSourceResultsConfig } from '../../Models/WebApiModels';
import Title from 'antd/es/typography/Title';
import { isNumber } from '../../Services/Validation';

export type SingleSourceResultsConfigurationDialogProps = {
    printMap?: boolean;
    highlightValue?: number;
    acceptableError?: number;
    includeGeoTiffData?: boolean;
};

@observer
export class SingleSourceResultsConfigurationDialog extends React.PureComponent<SingleSourceResultsConfigurationDialogProps, object> implements IModalDialogContent<SingleSourceResultsConfig> {
    private _from: FormModel;

    constructor(props: SingleSourceResultsConfigurationDialogProps) {
        super(props);
        this._from = new FormModel();
        this._from.printMap = props.printMap ?? true;
        this._from.highlightValue = props.highlightValue;
        this._from.acceptableError = props.acceptableError;
        this._from.includeGeoTiffData = props.includeGeoTiffData ?? false; 
    }

    public getModalOptions(window: ModalWindow<SingleSourceResultsConfig>): ModalDialogOptions<SingleSourceResultsConfig> {
        return {
            title: 'Настройка результатов',
            buttons: [
                {
                    type: ModalButtonType.Cancel,
                    title: 'Отмена',
                    onClick: () => {
                        window.close(ModalButtonType.Cancel);
                    }
                },
                {
                    type: ModalButtonType.Save,
                    title: 'Сохранить',
                    isDisabled: !this._from.isFormValid,
                    onClick: () => {
                        window.close(ModalButtonType.Save, {
                            printMap: this._from.printMap,
                            highlightValue: this._from.highlightValue,
                            acceptableError: this._from.acceptableError,
                            includeGeoTiffData: this._from.includeGeoTiffData,
                        });
                    }
                }
            ],
            width: '640px'
        };
    }

    render() {
        return (
            <Col>
                <Title level={5}>GeoTiff</Title>
                <FormCheckbox formModel={this._from} name="printMap" checked={this._from.printMap} title="Наложить на карту" />
                <FormCheckbox formModel={this._from} name="includeGeoTiffData" checked={this._from.includeGeoTiffData} title="Отчет со значениями" />
                <Space direction="horizontal">
                    <Col>
                        <FormInput formModel={this._from} name="highlightValue" label="Выделить значение" addonAfter="Cm" hideValidationErrors />
                    </Col>
                    <Col>
                        <FormInput formModel={this._from} name="acceptableError" label="Ошибка" addonAfter="Cm" hideValidationErrors />
                    </Col>
                </Space>
            </Col>
        );
    }
}

class FormModel extends BaseFormModel {
    @observable
    public accessor printMap: boolean = true;

    @observable
    public accessor includeGeoTiffData: boolean = false;

    @observable
    @isNumber()
    public accessor highlightValue: number | undefined;

    @observable
    @isNumber()
    public accessor acceptableError: number | undefined;
}
