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
import { isNumber } from "../../Services/Validation";

class FormModel extends BaseFormModel {
    @isNumber()
    @observable
    public accessor a: number | undefined;

    @observable
    public accessor h: number | undefined;

    @observable
    public accessor f: number | undefined;

    @observable
    public accessor airTemperature: number | undefined;

    @observable
    public accessor emissionTemperature: number | undefined;

    @observable
    public accessor m: number | undefined;

    @observable
    public accessor d: number | undefined;

    @observable
    public accessor eta: number | undefined;

    @observable
    public accessor w: number | undefined;

    @observable
    public accessor u: number | undefined;

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
                        changeHandler={() => this._onAnyFieldChange()}/>

                    <Tooltip trigger={['focus']} title="Масса ЗВ, выбрасываемого в атмосферный воздух в единицу времени (мощность выброса), г/с" placement="topLeft">
                        <Input placeholder="M" style={{ width: '80px' }} value={this._form.m} onChange={(v) => this._form.m = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Безразмерный коэффициент, учитывающий скорость оседания ЗВ (газообразных и аэрозолей, включая твердые частицы) в атмосферном воздухе" placement="topLeft">
                        <Input placeholder="F" style={{ width: '80px' }} value={this._form.f} onChange={(v) => this._form.f = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Безразмерный коэффициент, учитывающий влияние рельефа местности" placement="topLeft">
                        <Input placeholder="η" style={{ width: '80px' }} value={this._form.eta} onChange={(v) => this._form.eta = +v.target.value} />
                    </Tooltip>
                </div>
                <div className="d-flex flex-row mt-3" style={{ gap: '20px' }}>
                    <Tooltip trigger={['focus']} title="Высота источника выброса, м" placement="topLeft">
                        <Input placeholder="H" style={{ width: '80px' }} value={this._form.h} onChange={(v) => this._form.h = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Диаметр устья источника выброса, м" placement="topLeft">
                        <Input placeholder="D" style={{ width: '80px' }} value={this._form.d} onChange={(v) => this._form.d = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Температура выбрасываемой ГВС, °C" placement="topLeft">
                        <Input placeholder="Tг" style={{ width: '80px' }} value={this._form.emissionTemperature} onChange={(v) => this._form.emissionTemperature = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Температурой атмосферного воздуха, °C" placement="topLeft">
                        <Input placeholder="Tв" style={{ width: '80px' }} value={this._form.airTemperature} onChange={(v) => this._form.airTemperature = +v.target.value} />
                    </Tooltip>
                </div>
                <div className="d-flex flex-row mt-3 mb-3" style={{ gap: '20px' }}>
                    <Tooltip trigger={['focus']} title="Средняя скорость выхода ГВС из устья источника выброса, м/с" placement="topLeft">
                        <Input placeholder="W" style={{ width: '80px' }} value={this._form.w} onChange={(v) => this._form.w = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Скорость ветра" placement="topLeft">
                        <Input placeholder="U" style={{ width: '80px' }} value={this._form.u} onChange={(v) => this._form.u = +v.target.value} />
                    </Tooltip>

                    <Tooltip trigger={['focus']} title="Расстояние" placement="topLeft">
                        <Input placeholder="X" style={{ width: '80px' }} value={this._form.x} onChange={(v) => this._form.x = +v.target.value} />
                    </Tooltip>

                    <Button type="primary" style={{ width: '80px', padding: '0px' }} onClick={() => this._calculate()}>Рассчитать</Button>
                </div>
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
                    </div>)
            }
        </>
    }

    @action
    private _onAnyFieldChange() {
        this._form.validate();
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
}