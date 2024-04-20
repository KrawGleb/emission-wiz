import React from "react";
import { observer } from "mobx-react";
import { Radar } from "react-chartjs-2";
import { computed, observable, runInAction } from "mobx";
import { BaseFormModel } from "../Models/BaseFromModel";
import { Button, Col, Row, Space } from "antd";
import { FormInput } from "./FormControls";
import { PlusOutlined } from "@ant-design/icons";

export type WindDirection = {
	label?: string;
	degree: number;
	speed?: number;
};

const predefinedDirections: WindDirection[] = [
	{
		label: "С",
		degree: 0,
		speed: 2,
	},
	{
		label: "СВ",
		degree: 45,
	},
	{
		label: "В",
		degree: 90,
		speed: 2,
	},
	{
		label: "ЮВ",
		degree: 135,
	},
	{
		label: "Ю",
		degree: 180,
		speed: 2,
	},
	{
		label: "ЮЗ",
		degree: 225,
	},
	{
		label: "З",
		degree: 270,
		speed: 2,
	},
	{
		label: "СЗ",
		degree: 315,
	},
];

const labels = Array(360)
	.fill("")
	.map((_x, i) => {
		const predefinedDirection = predefinedDirections.find(
			(t) => t.degree === i
		);
		return predefinedDirection ? predefinedDirection.label : "";
	});

@observer
export default class WindRose extends React.Component {
	@observable private accessor _forms: FormModel[] = [];

	constructor(props: object) {
		super(props);

		this._forms = predefinedDirections
			.filter((x) => x.degree.toString().endsWith("0"))
			.map((x) => new FormModel(x.label, x.degree, x.speed ?? 0));
	}

	render() {
		return (
			<Row>
				<Col>
					<Radar data={this.data} />
				</Col>
				<Col>
					{this._forms.map((x) => this._renderForm(x))}
					<Row
						style={{ flexDirection: "row-reverse" }}
						onClick={() =>
							runInAction(() =>
								this._forms.push(new FormModel(undefined, 0, 2))
							)
						}
					>
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
					<Col style={{ marginBottom: "24px", width: "20px" }}>
						{form.label}
					</Col>
					<Col>
						<FormInput
							formModel={form}
							name="degree"
							style={{ width: "80px" }}
							suffix="°"
						/>
					</Col>
					<Col>
						<FormInput
							formModel={form}
							name="speed"
							style={{ width: "80px" }}
							suffix="м/с"
						/>
					</Col>
				</Space>
			</Row>
		);
	}

	@computed
	get data() {
		return {
			labels: labels,
			datasets: [
				{
					label: "Скорость ветра м/с",
					data: Array(360)
						.fill(1)
						.map((_x, i) => {
							const form = this._forms.find(
								(x) => +x.degree === i
							);
							return form ? form.speed : 0;
						}),
					backgroundColor: "rgba(255, 99, 132, 0.2)",
					borderColor: "rgba(255, 99, 132, 1)",
					borderWidth: 1,
				},
			],
		};
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
	@observable accessor degree: number;
	@observable accessor speed: number;
}
