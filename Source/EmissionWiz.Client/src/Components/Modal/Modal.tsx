import React, { ClassType, FunctionComponent } from 'react';
import { observable, action, computed, makeObservable } from 'mobx';
import { observer } from 'mobx-react';
import { AxiosError } from 'axios';
import { Button, Modal } from 'antd';
import Draggable from 'react-draggable';

import { PromiseCompletion } from '../../Classes/PromiseCompletion';
import { InformationDialog } from '../Dialogs/InformationDialog';
import { ErrorDialog } from '../Dialogs/ErrorDialog';
import { ConfirmationDialog } from '../Dialogs/ConfirmationDialog';
import { Loading } from '../Loading/Loading';

export type ButtonColor = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark' | 'link';
export type ButtonType = 'primary' | 'link' | 'text' | 'default' | 'dashed';

type ClickHandler<T> = (window: ModalWindow<T>) => void | Promise<void>;

export type ModalButtonOptions<T> = {
    type: ModalAction;
    color?: ButtonColor;
    title?: string;
    outline?: boolean;
    isDisabled?: boolean;
    setFocus?: boolean;
    alignLeft?: boolean;
    children?: ModalButtonOptions<T>[];
    onClick: ClickHandler<T>;
};

export type ModalOptions<T> = {
    title?: string;
    width?: number | string;
    closeByEscape?: boolean;
    dialog: (window: ModalWindow<T>) => JSX.Element;
    buttons?: (window: ModalWindow<T>) => ModalButtonOptions<T>[];
    onAppear?: (window: ModalWindow<T>) => void;
    onHide?: (window: ModalWindow<T>) => void;
};

export enum ModalButtonType {
    Ok = 'Ok',
    Cancel = 'Cancel',
    Close = 'Close',
    Save = 'Save',
    Update = 'Update',
    Remove = 'Remove',
    Confirm = 'Confirm',
    Reset = 'Reset',
    Add = 'Add',
    CreateNew = 'CreateNew',
    Send = 'Send',
    Delete = 'Delete',
    Yes = 'Yes',
    No = 'No',
    Submit = 'Submit',
    Apply = 'Apply',
    Download = 'Download',
    RunNow = 'RunNow',
    SaveAndRunNow = 'SaveAndRunNow'
}

type ConfirmationOptions = {
    title?: string;
    color?: ButtonColor;
};

type ModalAction = ModalButtonType | string;

export type ModalResult<T> = {
    button: ModalAction;
    result: T | null;
};

type ModalWindowProps<T> = ModalOptions<T> & {
    onClose: (action: ModalAction, result?: T) => void;
};

type ModalPromise<T> = Promise<ModalResult<T>> & {
    close: (action?: ModalAction, result?: T) => void;
};

export type ModalDialogOptions<T> = {
    buttons?: ModalButtonOptions<T>[];
    loader?: PromiseCompletion;
    title?: string;
    width?: number | string;
    modalClassName?: string;
    bodyClassName?: string;
    disableAutoFocus?: boolean;
    footerPrefix?: JSX.Element;
};

export interface IModalDialogContent<T> {
    getModalOptions: (window: ModalWindow<T>) => ModalDialogOptions<T>;
}

@observer
export class Modals extends React.Component<object> {
    render() {
        return <>{modalService.renderModals()}</>;
    }
}

type ModalButtonProps<T> = ModalButtonOptions<T> & {
    window: ModalWindow<T>;
    innerRef?: React.Ref<HTMLButtonElement>;
};

@observer
class ModalButton<T> extends React.Component<ModalButtonProps<T>, object> {
    @observable private accessor _isWorking: boolean = false;

    constructor(props: ModalButtonProps<T>) {
        super(props);
        makeObservable(this);
    }

    render() {
        const { title, isDisabled, type, onClick } = this.props;

        let buttonType: ButtonType | undefined = undefined;
        let isDanger = false;
        if (type === ModalButtonType.Add || type === ModalButtonType.Save || type === ModalButtonType.Update || type === ModalButtonType.Download) {
            buttonType = 'primary';
        }
        if (type === ModalButtonType.Yes || type === ModalButtonType.Apply) {
            buttonType = 'primary';
        }
        if (type === ModalButtonType.Remove || type === ModalButtonType.No || type === ModalButtonType.Reset) {
            isDanger = true;
        }

        return (
            <Button className={this._isWorking ? 'btn-modal-loader' : ''} type={buttonType} danger={isDanger} disabled={isDisabled || this._isWorking} onClick={() => this._onClick(onClick)}>
                {title ?? type}
            </Button>
        );
    }

    private async _onClick(onClick: ClickHandler<T>) {
        const { window } = this.props;
        const result = onClick(window);
        if (result) {
            this._isWorking = true;
            try {
                await result;
            } finally {
                this._isWorking = false;
            }
        }
    }
}

@observer
export class ModalWindow<T> extends React.Component<ModalWindowProps<T>, object> {
    private _payload: T | null = null;
    private _focusButton: React.RefObject<HTMLButtonElement> = React.createRef();
    @observable public accessor contentRef: React.RefObject<IModalDialogContent<T>> = React.createRef();

    constructor(props: ModalWindowProps<T>) {
        super(props);
        makeObservable(this);
    }

    componentDidMount() {
        document.addEventListener('keydown', this._onDocumentKeyDown);
        this.props.onAppear?.(this);
        window.setTimeout(() => this._focusButton.current && this._focusButton.current.focus(), 0);
    }

    componentWillUnmount() {
        this.props.onHide?.(this);
        document.removeEventListener('keydown', this._onDocumentKeyDown);
    }

    render() {
        const { title, dialog, buttons, width } = this.props;
        const content = this.contentRef.current as IModalDialogContent<T>;

        const modalOptions = content?.getModalOptions?.call(content, this);

        const titleToRender = title ?? modalOptions?.title;
        const maxWidth = width ?? modalOptions?.width;
        const widthToRender = typeof maxWidth === 'number' ? `${maxWidth}px` : maxWidth;
        const contentButtons = modalOptions?.buttons ?? [];
        const loader = modalOptions?.loader;
        const buttonsToRender = (buttons?.(this) ?? []).concat(contentButtons);
        const leftButtons = buttonsToRender.filter((b) => !!b.alignLeft);
        const normalButtons = buttonsToRender.filter((b) => !b.alignLeft);
        const footerPrefix = modalOptions?.footerPrefix;
        return (
            <Draggable handle=".ant-modal-header">
                <Modal
                    title={titleToRender}
                    className={modalOptions?.modalClassName}
                    open={true}
                    style={{ maxWidth: widthToRender }}
                    onCancel={() => this._onCloseClick()}
                    footer={
                        buttonsToRender.length || footerPrefix
                            ? [
                                  <>
                                      {footerPrefix}
                                      {leftButtons.length ? <div className="modal-footer-left">{this._renderButtons(leftButtons, modalOptions)}</div> : null}
                                      {this._renderButtons(normalButtons, modalOptions)}
                                  </>
                              ]
                            : []
                    }
                >
                    <React.Suspense fallback={<Loading isSuspense />}>{dialog(this)}</React.Suspense>
                    <Loading loading={loader?.isPending} />
                </Modal>
            </Draggable>
        );
    }

    private _renderButtons(buttons: ModalButtonOptions<T>[], options?: ModalDialogOptions<T>) {
        return buttons.map((b, index) => {
            let isFocused = buttons.length === 1 && !options?.disableAutoFocus;
            if (typeof b.setFocus === 'boolean') {
                isFocused = b.setFocus;
            } else if (!isFocused && !options?.disableAutoFocus) {
                const activeButtons = buttons.filter((b) => b.type !== ModalButtonType.Cancel && b.type !== ModalButtonType.Close);
                if (activeButtons.length === 1) {
                    isFocused = activeButtons[0].type === b.type;
                }
            }
            return <ModalButton<T> innerRef={isFocused ? this._focusButton : void 0} key={`${b.type}${index}`} {...b} window={this} />;
        });
    }

    @action.bound
    private _onDocumentKeyDown(event: KeyboardEvent) {
        const VK_ESCAPE = 27;
        const { closeByEscape } = this.props;
        if (event.keyCode === VK_ESCAPE && closeByEscape !== false) {
            this.props.onClose(ModalButtonType.Cancel, undefined);
        }
    }

    @action.bound
    private _onCloseClick() {
        this.props.onClose(ModalButtonType.Cancel, undefined);
    }

    public close(button: ModalAction = ModalButtonType.Close, result?: T) {
        this.props.onClose(button, result || this._payload || void 0);
    }

    public getPayload(): T | null {
        return this._payload;
    }

    public setPayload(payload: T | null) {
        this._payload = payload;
    }
}

class ModalService {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    @observable private accessor _modalData: ModalWindowProps<any>[] = [];

    constructor() {
        makeObservable(this);
    }

    @computed
    public get hasActiveModal() {
        return !!this._modalData.length;
    }

    public renderModals = () => {
        return this._modalData.map((m, index) => <ModalWindow key={'m' + index} {...m} />);
    };

    public show<P extends object, T>(dialog: ClassType<P, React.Component<P, object, object>, React.ComponentClass<P, object>> | FunctionComponent<P>, props?: P, options?: Omit<ModalDialogOptions<T>, 'dialog'>): ModalPromise<T> {
        return this.showModal(
            Object.assign(
                {},
                {
                    dialog: (window: ModalWindow<T>) => {
                        const content = React.createElement<P>(dialog, Object.assign({}, props, { ref: window.contentRef }));
                        return content;
                    }
                },
                options
            ) as ModalOptions<T>
        );
    }

    public showModal<T>(options: ModalOptions<T>): ModalPromise<T> {
        let data: ModalWindowProps<T> | null = null;
        const result = new Promise<ModalResult<T>>((resolve) => {
            data = Object.assign(options, {
                onClose: (button: ModalAction, result?: T) => {
                    resolve({
                        button: button,
                        result: result || null
                    });
                    const modalIndex = this._modalData.findIndex((d) => d === data);
                    if (modalIndex !== -1) {
                        this._modalData.splice(modalIndex, 1);
                    }
                    if (!this._modalData.length) {
                        document.body.classList.remove('modal-open');
                    }
                }
            });
            if (!this._modalData.length) {
                document.body.classList.add('modal-open');
            }
            this._modalData.push(data);
            data = this._modalData[this._modalData.length - 1];
        });

        return Object.assign(result, {
            close: (action?: ModalAction, result?: T) => data?.onClose(action || ModalButtonType.Ok, result)
        });
    }

    public async showConfirmation(message: string | string[] | JSX.Element, titleOrOptions?: string | ConfirmationOptions, options?: ConfirmationOptions) {
        const confirmationOptions = typeof titleOrOptions === 'object' ? titleOrOptions : options;
        const result = await this.show(ConfirmationDialog, {
            message: message,
            title: typeof titleOrOptions === 'string' ? titleOrOptions : titleOrOptions?.title,
            color: confirmationOptions?.color || 'primary'
        });

        return result.button === ModalButtonType.Confirm;
    }

    public async showInformation(
        message: string | string[] | JSX.Element,
        title?: string,
        options?: {
            className?: string;
        }
    ) {
        await this.show(InformationDialog, {
            message: message,
            title: title,
            className: options?.className
        });
    }

    public showError = async (errorContent: string | string[] | JSX.Element | AxiosError) => {
        await this.show(ErrorDialog, {
            error: errorContent
        });
    };
}

export const modalService = new ModalService();
