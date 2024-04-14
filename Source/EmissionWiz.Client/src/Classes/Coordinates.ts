import { observable } from "mobx";

export class Coordinates {
    @observable
    public accessor lat: number;
    
    @observable
    public accessor lon: number;
}