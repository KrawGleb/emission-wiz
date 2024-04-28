import * as React from 'react';
import { IModalDialogContent, ModalButtonType, ModalDialogOptions, ModalWindow } from '../Modal/Modal';

type InformationDialogProps = {
    message?: string | string[] | JSX.Element;
    title?: string;
    className?: string;
};

export class InformationDialog extends React.PureComponent<InformationDialogProps> implements IModalDialogContent<void> {
    public getModalOptions (window: ModalWindow<void>): ModalDialogOptions<void> {
        return {
            title: this.props.title ?? 'Information',
            buttons: [
                {
                    type: ModalButtonType.Ok,
                    onClick: () => {
                        window.close();
                    },
                }],
            width: '600px',
            bodyClassName: this.props.className,
        };
    }

    render () {
        const { message } = this.props;

        if (typeof message === 'string' || Array.isArray(message)) {
            const messages = typeof message === 'string' ? [message] : message;

            return (
                <>
                    {messages && messages.length > 0 &&
                        <ul>
                            {messages.map(m => <li key={m}>{m}</li>)}
                        </ul>
                    }
                </>
            );
        }

        return message;
    }
}