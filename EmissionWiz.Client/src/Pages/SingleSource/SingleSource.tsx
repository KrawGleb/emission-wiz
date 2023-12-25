import React from "react";
import { BaseFormModel } from "../../Models/BaseFromModel";
import { observable } from "mobx";
import { observer } from "mobx-react";
import { Report } from "../../Components/Report";

class FormModel extends BaseFormModel {
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
    render() {
        return (
            <Report title="Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника">

            </Report>
        )
    }
}