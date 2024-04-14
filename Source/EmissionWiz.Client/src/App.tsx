import React from 'react';
import { navigationStore } from './Stores/NavigationStore';
import AppRoutes from './routes';

export default class App extends React.Component<{}> {

    componentDidMount() {
        navigationStore.init();
    }

    render() {
        return <>
            {AppRoutes.routes}
        </>
    }
}
