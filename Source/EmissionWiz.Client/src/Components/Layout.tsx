import React from 'react';
import { Modals } from './Modal/Modal';

type LayoutProps = {
    children?: React.ReactNode;
};

export class Layout extends React.Component<LayoutProps, object> {
    render(): React.ReactNode {
        return (
            <>
                <Modals />
                <div className="content-wrapper" key="wrapper">
                    {this.props.children}
                </div>
            </>
        );
    }
}
