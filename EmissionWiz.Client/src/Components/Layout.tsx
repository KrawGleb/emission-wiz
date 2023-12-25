import React from "react";

type LayoutProps = {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    render(): React.ReactNode {
        return <div className="content-wrapper" key="wrapper">
            {this.props.children}
        </div>
    }
}