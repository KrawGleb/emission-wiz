import React from 'react';
import { navigationStore } from './Stores/NavigationStore';
import AppRoutes from './routes';
import { Chart as ChartJS, Filler, Legend, LineElement, PointElement, RadialLinearScale, Tooltip } from 'chart.js';

ChartJS.register(
    RadialLinearScale,
    PointElement,
    LineElement,
    Filler,
    Tooltip,
    Legend
);

export default class App extends React.Component<object> {
    componentDidMount() {
        navigationStore.init();
    }

    render() {
        
        return <>
            {AppRoutes.routes}
        </>
    }
}
