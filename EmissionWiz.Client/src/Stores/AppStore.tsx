import { observable } from "mobx";

class AppStore {
    @observable accessor isAdaptive: boolean = false;
    @observable accessor bodyClassList: Set<string> = new Set([].slice.apply(document.body.classList));

}

export const appStore = new AppStore();