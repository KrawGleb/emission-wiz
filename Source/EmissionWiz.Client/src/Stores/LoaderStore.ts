import { action, observable } from 'mobx';
import { PromiseCompletion } from '../Classes/PromiseCompletion';

export type GlobalProgressData = {
    total: number;
    current: number;
    title: string;
};

class LoaderStore {
    @observable public accessor globalProgress: GlobalProgressData | null = null;
    public globalLoader: PromiseCompletion = new PromiseCompletion();

    @action.bound
    public updateGlobalProgress (progressData: GlobalProgressData) {
        this.globalProgress = {
            total: progressData.total,
            current: progressData.current,
            title: progressData.title,
        };
        if (progressData.total === progressData.current) {
            this.globalProgress = null;
        }
    }
}

export const loaderStore = new LoaderStore();
