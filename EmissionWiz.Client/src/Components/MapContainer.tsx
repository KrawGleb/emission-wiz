import MapLibreGL, { Map, MapMouseEvent, Marker } from "maplibre-gl";
import { action, computed, observable } from "mobx";
import { observer } from "mobx-react";
import React from "react";
import { BaseFormModel } from "../Models/BaseFromModel";

// TODO:
// 1. Get api key from backend
// 2. Adjust map size 
// 3. Get center by ip

export type MapContainerProps<T extends BaseFormModel> = {
    formModel: T;
    name: keyof T;
    multiple?: boolean;
    className?: string;

    onRemoveMarker?: (key?: string) => void;
}

export type UniqueMarker = Marker & {
    key?: string;
}

@observer
export default class MapContainer<T extends BaseFormModel> extends React.Component<MapContainerProps<T>, {}> {
    @observable
    private accessor _map: Map | undefined;

    private readonly _mapContainer: React.RefObject<HTMLDivElement> = React.createRef();

    @observable
    private accessor _mapMarkers: Marker[] = [];

    @observable
    private accessor _onRemoveMarker: ((key?: string) => void) | undefined;

    @observable
    private accessor _multiple: boolean = false;

    @observable
    private accessor _formModel: T | undefined;

    @observable
    private accessor _name: keyof T | undefined;

    constructor(props: MapContainerProps<T>) {
        super(props);

        this._onRemoveMarker = props.onRemoveMarker;
        this._multiple = props.multiple ?? false;
        this._formModel = props.formModel;
        this._name = props.name;
    }

    componentDidMount(): void {
        if (!this._mapContainer.current) return;

        this._map = new MapLibreGL.Map({
            container: this._mapContainer.current,
            style: 'https://maps.geoapify.com/v1/styles/osm-carto/style.json?apiKey=87fc891dca524646a859e8e4a202c495',
            zoom: 13,
            center: {
                lat: 52.1,
                lon: 23.65,
            },
        });

        if (this._map) {
            this._map.on('click', (e) => this._onMapClick(e))
        }
    }

    render() {
        return <div className={this.props.className} style={{ width: '100%', height: '33vh' }}>
            <div style={{ width: '100%', height: '100%' }} ref={this._mapContainer} />
        </div>
    }

    @action
    public updateMarkers() {
        this._mapMarkers.forEach((marker) => {
            marker.remove();
        })

        const markers = this.markers;
        this._mapMarkers = markers.map((value, index) => {
            const marker = (new MapLibreGL.Marker()
                .setLngLat(value._lngLat)
                .addTo(this._map!)) as UniqueMarker;

            this._mapMarkers.push(marker);

            marker.key = `${index + 1}`;

            return marker;
        })
    }

    @action
    private _onMapClick(event: MapMouseEvent) {
        if (this.markers.length && !this.props.multiple) {
            this._onRemoveMarker?.(this.markers[0].key);
        }

        const marker = (new MapLibreGL.Marker()
            .setLngLat(event.lngLat)
            .addTo(this._map!)) as UniqueMarker;

        this._mapMarkers.push(marker);

        marker.key = `${this.markers.length + 1}`;
        if (!this._multiple) {
            this._formModel?.setValue(this._name!, marker);
        }
        else {
            this._formModel?.pushValue(this._name!, marker);
        }
    }

    @computed
    get markers(): UniqueMarker[] {
        return this._formModel?.getValue(this._name!) as UniqueMarker[];
    }
}