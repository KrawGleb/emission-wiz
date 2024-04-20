import React from "react";
import { observer } from "mobx-react";
import { Radar } from "react-chartjs-2";

const data = {
    labels: Array(360).fill(''),
    datasets: [{
        label: 'Скорость ветра м/с',
        data: Array(360).fill(2),
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        borderColor: 'rgba(255, 99, 132, 1)',
        borderWidth: 1,
    }]
}

export type WindDirection = {
    label?: string;
    degree: number;
    speed?: number;
}

const predefindedDirections: WindDirection[] = [
    {
        label: 'С',
        degree: 0
    },
    {
        label: 'СВ',
        degree: 45
    },
    {
        label: 'В',
        degree: 90,
    },
    {
        label: 'ЮВ',
        degree: 135,
    },
    {
        label: 'Ю',
        degree: 180,
    },
    {
        label: 'ЮЗ',
        degree: 225,
    },
    {
        label: 'З',
        degree: 270,
    },
    {
        label: 'СЗ',
        degree: 315,
    }
]

@observer
export default class WindRose extends React.Component {



    render() {
        return <Radar data={data}/>
    }   
}