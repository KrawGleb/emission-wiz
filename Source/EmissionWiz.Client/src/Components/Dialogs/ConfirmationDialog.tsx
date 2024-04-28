import * as React from 'react';

import { ModalButtonType, ModalWindow, ModalDialogOptions, IModalDialogContent, ButtonColor } from '../Modal/Modal';

type ConfirmationDialogProps = {
    title?: string;
    message?: string | string[] | JSX.Element;
    color?: ButtonColor;
};

export class ConfirmationDialog extends React.PureComponent<ConfirmationDialogProps, object> implements IModalDialogContent<void> {

    public getModalOptions (window: ModalWindow<void>): ModalDialogOptions<void> {
        const { title, color } = this.props;
        return {
            title: title ?? 'Confirmation',
            buttons: [
                {
                    type: ModalButtonType.Cancel,
                    onClick: () => {
                        window.close();
                    },
                }, {
                    color: color ?? 'secondary',
                    type: ModalButtonType.Confirm,
                    onClick: () => {
                        window.close(ModalButtonType.Confirm);
                    },
                }],
            width: '640px',
        };
    }

    render () {
        const { message } = this.props;

        if (typeof message === 'string' || Array.isArray(message)) {
            const messages = typeof message === 'string' ? [message] : message;

            return (
                <>
                    {messages && messages.length > 0 &&
                        <ul className="list-unstyled white-space-pre">
                            {messages.map(m => <li key={m}>{m}</li>)}
                        </ul>
                    }
                </>
            );
        }

        return message;
    }
}