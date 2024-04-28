import { observable, computed } from 'mobx';

export enum CompletionType {
    //to track that at least one of operations is still pending
    Pending,
    //that operation was completed at least once fully
    //then completion is created, then is't not completed
    //and will be completed once at least one promise is finished *AND* 
    //at least once waiting queue is empty (=all tasks are done once)
    Completed
}

export class PromiseCompletion {
    @observable private accessor _promiseList: PromiseLike<void | object | unknown>[] = [];
    @observable private accessor _completedOnce: boolean = false;
    private _type: CompletionType;
    private _waiters: (() => void)[] = [];

    constructor (type: CompletionType = CompletionType.Pending) {
        this._type = type;
    }

    @computed
    get isCompleted () {
        let result = !this._promiseList.length;
        if (this._type === CompletionType.Completed) {
            result = this._completedOnce;
        }
        return result;
    }

    @computed
    get isPending () {
        return !!this._promiseList.length;
    }

    public add<T> (callback: () => PromiseLike<T>) {
        const promise = callback();
        this.subscribe(promise);
        return promise;
    }

    public subscribeRange (promises: PromiseLike<void | object>[]) {
        promises.forEach((p) => this.subscribe(p));
    }

    public subscribe (promise: PromiseLike<void | object | unknown>) {
        if (this._promiseList.indexOf(promise) !== -1) {
            throw new Error('Promise is already registered!');
        }

        this._promiseList.push(promise);
        promise.then(
            () => this._complete(promise),
            () => this._complete(promise)
        );
    }

    private _complete (promise: PromiseLike<void | object | unknown>) {
        window.setTimeout(() => {
            const index = this._promiseList.indexOf(promise);
            if (index === -1) throw new Error('Promise is not registered!');

            this._promiseList.splice(index, 1);
            this._completedOnce = this._completedOnce || !this._promiseList.length;

            if (!this._promiseList.length) {
                const waiters = this._waiters.slice(0);
                this._waiters.length = 0;
                waiters.forEach((w) => w());
            }
        }, 0);
    }

    public wait (forceWait?: boolean): Promise<void> {
        if (this._completedOnce && !forceWait) {
            return Promise.resolve();
        }

        if (this._completedOnce && forceWait && !this.isPending) {
            return Promise.resolve();
        }

        return new Promise((resolve) => {
            this._waiters.push(resolve);
        });
    }
}
