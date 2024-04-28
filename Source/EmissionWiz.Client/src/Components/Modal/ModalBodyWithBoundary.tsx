import * as React from 'react';
import { ModalBody, ModalBodyProps } from 'reactstrap';

type ErrorBoundaryState = {
    hasError: boolean;
    error: Error | null;
    componentStack: string | null;
};

export class ModalBodyWithBoundary extends React.Component<ModalBodyProps, ErrorBoundaryState> {

    constructor (props: ModalBodyProps) {
        super(props);
        this.state = {
            hasError: false,
            error: null,
            componentStack: null,
        };
    }

    componentDidCatch (error: Error, errorInfo: React.ErrorInfo) {
        this.setState({
            hasError: true,
            error: error,
            componentStack: errorInfo.componentStack || null,
        });
    }

    render () {
        if (this.state.hasError) {
            const error = this.state.error;
            const message: string = error ? error.message : 'Error';
            return (
                <div className="error-container">
                    <h2>Something went wrong</h2>
                    <p>Please contact support.</p>
                    <div className="stack-block">
                        <h3>{message}</h3>
                        The above error occurred in:
                        {this.state.componentStack}
                    </div>
                </div>
            );
        }
        return (<ModalBody {...this.props}>{this.props.children}</ModalBody>);
    }
}