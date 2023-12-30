import React from "react";
import { BaseFormModel } from "../../Models/BaseFromModel";
import { action, observable } from "mobx";
import { observer } from "mobx-react";
import { Report } from "../../Components/Report";
import { Button, Input, Space, Tooltip, TooltipProps } from 'antd';
import ApiService from "../../Services/ApiService";
import { ApiUrls } from "../../AppConstants/ApiUrls";
import { SingleSourceEmissionCalculationResult, SingleSourceInputModel } from "../../Models/WebApiModels";
import { FormInput } from "../../Components/FormControls";
import { displayName, isNumber } from "../../Services/Validation";
import { downloadService } from "../../Services/DownloadService";

class FormModel extends BaseFormModel {
    @isNumber()
    @observable
    public accessor a: number | undefined;

    @isNumber()
    @observable
    public accessor h: number | undefined;

    @isNumber()
    @observable
    public accessor f: number | undefined;

    @displayName("Тв")
    @isNumber()
    @observable
    public accessor airTemperature: number | undefined;

    @displayName("Тг")
    @isNumber()
    @observable
    public accessor emissionTemperature: number | undefined;

    @isNumber()
    @observable
    public accessor m: number | undefined;

    @isNumber()
    @observable
    public accessor d: number | undefined;

    @displayName(" η")
    @isNumber()
    @observable
    public accessor eta: number | undefined;

    @isNumber()
    @observable
    public accessor w: number | undefined;

    @isNumber()
    @observable
    public accessor u: number | undefined;

    @isNumber()
    @observable
    public accessor x: number | undefined;
}

@observer
export default class SingleSource extends React.Component<{}, {}> {
    @observable
    private accessor _calculationResult: SingleSourceEmissionCalculationResult | undefined;

    private _form: FormModel = new FormModel();

    render() {
        return (
            <Report title="Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника">
                <h4>Вводные данные:</h4>
                <div className="d-flex flex-row" style={{ gap: '20px' }}>
                    <FormInput
                        formModel={this._form}
                        name="a"
                        placeholder="A"
                        style={{ width: '80px' }}
                        value={this._form.a}
                        tooltip={{
                            trigger: 'focus',
                            title: "Коэффициент, зависящий от температурной стратификации атмосферы, определяющий условия горизонтального и вертикального рассеивания ЗВ в атмосферном воздухе",
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()} />

                    <FormInput
                        formModel={this._form}
                        name="m"
                        placeholder="M"
                        style={{ width: '80px' }}
                        value={this._form.m}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Масса ЗВ, выбрасываемого в атмосферный воздух в единицу времени (мощность выброса), г/с',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="f"
                        placeholder="F"
                        style={{ width: '80px' }}
                        value={this._form.f}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Безразмерный коэффициент, учитывающий скорость оседания ЗВ (газообразных и аэрозолей, включая твердые частицы) в атмосферном воздухе',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="eta"
                        placeholder="η"
                        style={{ width: '80px' }}
                        value={this._form.eta}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Безразмерный коэффициент, учитывающий влияние рельефа местности',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />
                </div>
                <div className="d-flex flex-row mt-3" style={{ gap: '20px' }}>
                    <FormInput
                        formModel={this._form}
                        name="h"
                        placeholder="H"
                        style={{ width: '80px' }}
                        value={this._form.h}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Высота источника выброса, м',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="d"
                        placeholder="D"
                        style={{ width: '80px' }}
                        value={this._form.d}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Диаметр устья источника выброса, м',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="emissionTemperature"
                        placeholder="Tг"
                        style={{ width: '80px' }}
                        value={this._form.emissionTemperature}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Температура выбрасываемой ГВС, °C',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="airTemperature"
                        placeholder="Tв"
                        style={{ width: '80px' }}
                        value={this._form.airTemperature}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Температурой атмосферного воздуха, °C',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />
                </div>
                <div className="d-flex flex-row mt-3 mb-3" style={{ gap: '20px' }}>
                    <FormInput
                        formModel={this._form}
                        name="w"
                        placeholder="W"
                        style={{ width: '80px' }}
                        value={this._form.w}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Средняя скорость выхода ГВС из устья источника выброса, м/с',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="u"
                        placeholder="U"
                        style={{ width: '80px' }}
                        value={this._form.u}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Скорость ветра, м/с',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <FormInput
                        formModel={this._form}
                        name="x"
                        placeholder="X"
                        style={{ width: '80px' }}
                        value={this._form.x}
                        tooltip={{
                            trigger: 'focus',
                            title: 'Расстояние, м',
                            placement: 'topLeft'
                        } as TooltipProps}
                        changeHandler={() => this._onAnyFieldChange()}
                    />

                    <Button type="primary" style={{ width: '80px', padding: '0px' }} onClick={() => this._calculate()}>Рассчитать</Button>
                </div>
                <Button type="primary" onClick={() => this._fillWithTestData()}>Заполнить тестовыми данными</Button>
                {this._renderResult()}
            </Report>
        )
    }

    private _renderResult() {
        return <>
            {
                this._calculationResult && (
                    <div>
                        <h4>Результаты вычислений:</h4>
                        <span>Максимальная приземная разовая концентрация ЗВ: {this._calculationResult.cm.toFixed(4)}</span><br />
                        <span>Опасная скорость ветра: {this._calculationResult.um.toFixed(4)}</span><br />
                        <span>Максимальная приземная концентрация ЗВ: {this._calculationResult.cmu.toFixed(4)}</span><br />
                        <span>Расстояние от источника выброса, на котором при скорости ветра при неблагоприятных метеорологических условиях достигается максимальная приземная концентрация ЗВ: {this._calculationResult.xmu.toFixed(4)}</span><br />
                        <span>Приземная концентрация ЗВ: {this._calculationResult.c.toFixed(4)}</span><br />
                        <Button type="link" onClick={() => this._downloadReport()}>Ход вычислений</Button>
                    </div>)
            }
        </>
    }

    @action
    private _onAnyFieldChange() {
        this._form.validate();
    }

    @action
    private _fillWithTestData() {
        this._form.a = 32;
        this._form.m = 12;
        this._form.h = 40;
        this._form.d = 10;
        this._form.airTemperature = 17;
        this._form.emissionTemperature = 25;
        this._form.eta = 2;
        this._form.f = 2;
        this._form.x = 200;
        this._form.u = 20;
        this._form.w = 20;
    }

    @action.bound
    private async _calculate() {
        const model: SingleSourceInputModel = {
            a: this._form.a!,
            m: this._form.m!,
            fCoef: this._form.f!,
            h: this._form.h!,
            d: this._form.d!,
            w: this._form.w!,
            eta: this._form.eta!,
            airTemperature: this._form.airTemperature!,
            emissionTemperature: this._form.emissionTemperature!,
            u: this._form.u!,
            x: this._form.x!,
        }
        const { data } = await ApiService.postTypedData<SingleSourceEmissionCalculationResult>(ApiUrls.SingleSource, model);
        this._calculationResult = data;
    }

    @action.bound
    private async _downloadReport() {
        const model: SingleSourceInputModel = {
            a: this._form.a!,
            m: this._form.m!,
            fCoef: this._form.f!,
            h: this._form.h!,
            d: this._form.d!,
            w: this._form.w!,
            eta: this._form.eta!,
            airTemperature: this._form.airTemperature!,
            emissionTemperature: this._form.emissionTemperature!,
            u: this._form.u!,
            x: this._form.x!,
        }
        downloadService.downloadFile(ApiUrls.SingleSourceReport, model);
    }
}