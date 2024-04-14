import { observable } from "mobx";

class AppStore {
    @observable accessor isAdaptive: boolean = false;
    @observable accessor bodyClassList: Set<string> = new Set([].slice.apply(document.body.classList));

    notificationBar: HTMLElement | null = null;
    notificationBarMessage: HTMLElement | null = null;
}

export const appStore = new AppStore();