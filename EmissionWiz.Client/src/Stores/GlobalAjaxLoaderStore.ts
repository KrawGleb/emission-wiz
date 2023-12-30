import {action} from 'mobx';

class GlobalAjaxLoaderStore {
    private _actionElement: HTMLInputElement | HTMLButtonElement | null = null;
    private _actionElementMap: Map<number, HTMLInputElement | HTMLButtonElement> = new Map<number, HTMLInputElement | HTMLButtonElement>();

    public appLoaderElement?: HTMLElement;
    public appLoaderMessageElement?: HTMLElement;
    public startupLoaders: Promise<void>[] = [];

    constructor() {
        document.addEventListener('click', this.appClickHandler);
    }

    @action
    public hideAppLoader() {
        const loaderNode = this.appLoaderElement;
        const messageNode = this.appLoaderMessageElement;
        if (!loaderNode) return;

        if (!loaderNode.classList.contains('hide')) {
            loaderNode.classList.add('hide');
            if (messageNode) {
                messageNode.innerText = '';
            }
        }
    }

    @action
    public showAppLoader(message?: string) {
        const loaderNode = this.appLoaderElement;
        const messageNode = this.appLoaderMessageElement;
        if (!loaderNode) return;

        loaderNode.classList.remove('invisible');
        window.setTimeout(() => {
            loaderNode.classList.remove('hide');
            if (message && messageNode) {
                messageNode.innerText = message;
            }
        }, 0);
    }

    @action
    public showAjaxSpinner(timestamp: number) {
        const elem = this._actionElement;
        if (elem) {
            this._actionElementMap.set(timestamp, elem);
            elem.disabled = true;
            elem.classList.add('btn-loader');
            this._actionElement = null;
        }
    }

    @action
    public hideAjaxSpinner(timestamp: number) {
        const elem = this._actionElementMap.get(timestamp);
        if (elem) {
            elem.disabled = false;
            elem.classList.remove('btn-loader');
            this._actionElementMap.delete(timestamp);
        }
    }

    @action.bound
    public appClickHandler(event: MouseEvent) {
        this.setActiveAjaxElement(event.target);
    }

    @action
    public setActiveAjaxElement(target: EventTarget | null) {
        const element = target as HTMLInputElement | HTMLButtonElement | null;
        const button = element && element.closest('button');
        if (button && !button.classList.contains('no-loader')) {
            this._actionElement = button;
        }
    }
}

export const globalAjaxLoaderStore = new GlobalAjaxLoaderStore();
