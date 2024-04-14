import * as h from 'history';
import { action, observable } from 'mobx';

export const history: h.BrowserHistory = h.createBrowserHistory()

class NavigationStore {
    @observable public accessor currentLocation: h.Location;

    constructor() {
        this.currentLocation = Object.create(history.location)
    }

    @action
    public init() {
        history.listen(location => {
            this.currentLocation = location.location;
        });
    }
}

export const navigationStore = new NavigationStore();