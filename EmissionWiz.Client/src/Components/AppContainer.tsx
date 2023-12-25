import React from "react";
import { observer } from 'mobx-react';
import { autorun, IReactionDisposer } from 'mobx';
import { appStore } from "../Stores/AppStore";

export type AppContainerProps = {
    children?: React.ReactNode;
};

@observer
export class AppContainer extends React.Component<AppContainerProps, {}> {
    private _autoDisposer: IReactionDisposer;
    private _isAdaptive: boolean = false;
    private _bodyClass: string = '';

    updateViewPort = () => {
        const responsiveSwitchBreakpoint = 1024;
        const viewportNode = document.querySelector('meta[name="viewport"]');
        const documentNode = document.documentElement;
        if (viewportNode && documentNode) {
            let content = 'width=device-width, initial-scale=1.0';
            if (!appStore.isAdaptive && documentNode.clientWidth < responsiveSwitchBreakpoint) {
                content = 'width=' + responsiveSwitchBreakpoint + ', initial-scale=' + documentNode.clientWidth / responsiveSwitchBreakpoint;
            }
            viewportNode.setAttribute('content', content);
        }
    };

    updateBodyClass() {
        if (this._bodyClass !== [...appStore.bodyClassList].join(' ')) {
            this._bodyClass = [...appStore.bodyClassList].join(' ');
            document.body.className = this._bodyClass;
        }
    }

    componentDidMount() {
        this.updateViewPort();
        this.updateBodyClass();
        this._autoDisposer = autorun(() => {
            if (this._isAdaptive !== appStore.isAdaptive) {
                this.updateViewPort();
                this._isAdaptive = appStore.isAdaptive;
            }
            this.updateBodyClass();
        });
    }

    componentWillUnmount() {
        this._autoDisposer();
    }

   render() {
       return this.props.children;
   }
}