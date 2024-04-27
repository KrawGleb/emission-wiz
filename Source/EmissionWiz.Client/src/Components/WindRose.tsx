import React from 'react';
import { observer } from 'mobx-react';
import { Radar } from 'react-chartjs-2';
import { action, observable, runInAction } from 'mobx';
import { BaseFormModel } from '../Models/BaseFromModel';
import { Button, Col, Row, Space } from 'antd';
import { FormInput } from './FormControls';
import { PlusOutlined } from '@ant-design/icons';
import { SplineData } from '../Models/WebApiModels';
import ApiService from '../Services/ApiService';
import { ChartData } from 'chart.js';
import { isNumber } from '../Services/Validation';

export type WindDirection = {
    label?: string;
    degree: number;
    speed?: number;
};

const predefinedDirections: WindDirection[] = [
    {
        label: 'С',
        degree: 0,
        speed: 2
    },
    {
        label: 'СВ',
        degree: 45
    },
    {
        label: 'В',
        degree: 90,
        speed: 2
    },
    {
        label: 'ЮВ',
        degree: 135
    },
    {
        label: 'Ю',
        degree: 180,
        speed: 2
    },
    {
        label: 'ЮЗ',
        degree: 225
    },
    {
        label: 'З',
        degree: 270,
        speed: 2
    },
    {
        label: 'СЗ',
        degree: 315
    }
];

const labels = Array(360)
    .fill('')
    .map((_x, i) => {
        const predefinedDirection = predefinedDirections.find((t) => t.degree === i);
        return predefinedDirection ? predefinedDirection.label : '';
    });

export type WindRoseProps<T extends BaseFormModel> = {
	formModel: T;
	name: keyof T;
}

@observer
export default class WindRose<T extends BaseFormModel> extends React.Component<WindRoseProps<T>> {
    @observable private accessor _forms: FormModel[] = [];
    @observable private accessor _spline: SplineData | undefined;
    @observable private accessor _data: ChartData<'radar', number[], string | undefined> | undefined;

    constructor(props: WindRoseProps<T>) {
        super(props);

        this._forms = predefinedDirections.filter((x) => x.degree.toString().endsWith('0')).map((x) => new FormModel(x.label, x.degree, x.speed ?? 0));
        void this._respline();
    }

    render() {
        return (
            <Row>
                {this._data ? (
                    <Col>
                        <Radar data={this._data} />
                    </Col>
                ) : (
                    <>Loading...</>
                )}
                <Col>
                    {this._forms.map((x) => this._renderForm(x))}
                    <Row style={{ flexDirection: 'row-reverse' }} onClick={() => runInAction(() => this._forms.push(new FormModel(undefined, 0, 2)))}>
                        <Button type="primary" icon={<PlusOutlined />}>
                            Add
                        </Button>
                    </Row>
                </Col>
            </Row>
        );
    }

    private _renderForm(form: FormModel) {
        return (
            <Row>
                <Space>
                    <Col style={{ marginBottom: '24px', width: '20px' }}>{form.label}</Col>
                    <Col>
                        <FormInput formModel={form} name="degree" style={{ width: '80px' }} suffix="°" changeHandler={() => void this._respline()} />
                    </Col>
                    <Col>
                        <FormInput formModel={form} name="speed" style={{ width: '80px' }} suffix="м/с" changeHandler={() => void this._respline()} />
                    </Col>
                </Space>
            </Row>
        );
    }

    @action.bound
    private async _respline() {
        const xs = this._forms
            .slice()
            .sort((a, b) => a.degree - b.degree)
            .map((x) => x.degree);
        const ys = this._forms
            .slice()
            .sort((a, b) => a.degree - b.degree)
            .map((x) => x.speed);
        const xsCont = xs.map((x) => +x + 360);

        const splineData = {
            xs: [...xs, ...xsCont],
            ys: [...ys, ...ys],
            count: 360 * 2
        };

        const { data } = await ApiService.postTypedData<SplineData>('/math/spline', splineData);
        this._spline = data;

        this._data = {
            labels: labels,
            datasets: [
                {
                    label: 'Скорость ветра м/с',
                    data: Array(360)
                        .fill(1)
                        .map((_x, i) => {
                            const form = this._forms.find((x) => +x.degree === i);
                            if (form) {
                                return form.speed;
                            } else if (this._spline) {
                                const distances = this._spline?.xs.map((x) => Math.abs(x - i));
                                const minDistance = Math.min(...distances);
                                const minDistanceIndex = distances.findIndex((x) => x === minDistance);

                                const closest = this._spline.ys[minDistanceIndex];
                                return closest;
                            }

                            return 0;
                        }),
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                }
            ]
        };

		this.props.formModel.setValue(this.props.name, this._forms.map(x => ({
			degree: x.degree,
			speed: x.speed,
			label: x.label
		} as WindDirection)))
    }
}

class FormModel extends BaseFormModel {
    constructor(label?: string, degree?: number, speed?: number) {
        super();

        if (label !== undefined) {
            this.label = label;
        }

        if (degree !== undefined) {
            this.degree = +degree;
        }

        if (speed !== undefined) {
            this.speed = speed;
        }
    }

    @observable accessor label: string | undefined;

	
    @observable 
	@isNumber()
	accessor degree: number;

    @observable 
	@isNumber()
	accessor speed: number;
}
