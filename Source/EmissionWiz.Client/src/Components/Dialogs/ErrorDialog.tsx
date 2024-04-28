import * as React from 'react';
import { AxiosError } from 'axios';

import { ModalWindow, ModalDialogOptions, IModalDialogContent, ModalButtonType } from '../Modal/Modal';

type ErrorDialogProps = {
    error?: string[] | string | AxiosError | JSX.Element;
};

export class ErrorDialog extends React.PureComponent<ErrorDialogProps, object> implements IModalDialogContent<void> {

    public getModalOptions (window: ModalWindow<void>): ModalDialogOptions<void> {
        return {
            title: this.props.error ? `Request can't be completed` : `Error`,
            buttons: [
                {
                    type: ModalButtonType.Ok,
                    onClick: () => {
                        window.close();
                    },
                }],
            width: '500px',
        };
    }

    render () {
        return (
            <>
                {this._renderMessage()}
                {this._renderRequestMessage()}
            </>
        );
    }

    _renderMessage () {
        const { error } = this.props;

        if (typeof error === 'string') {
            return <p>{error}</p>;
        }

        if (Array.isArray(error)) {
            return (
                <>
                    {error.map((mess, index) => <p key={index}>{mess}</p>)}
                </>
            );
        }

        if (!React.isValidElement(error)) return null;

        return error;
    }

    _renderRequestMessage () {
        const { error } = this.props;

        if (!error) return null;
        if (typeof error !== 'object') return null;
        if (Array.isArray(error)) return null;
        if (React.isValidElement(error)) return null;

        const axiosError = error as AxiosError;
        if (!axiosError.response) {
            return null;
        }

        const errorData = axiosError.response.data as { error?: string };
        return (
            <>
                {errorData &&
                    <p>{Array.isArray(errorData) ? errorData.join(' ') : typeof errorData === 'string' ? errorData : errorData.error}</p>}
                <div><small>Code: {axiosError.response.status}</small></div>
                <div><small>URL: {axiosError.config && axiosError.config.url}</small></div>
            </>
        );
    }
}