import MapLibreGL, { Map, Marker } from "maplibre-gl";
import { observable } from "mobx";
import { observer } from "mobx-react";
import React from "react";

// TODO:
// 1. Get api key from backend
// 2. Adjust map size 
// 3. Get center by ip

export type MapContainerProps = {
    onClick?: (lng: number, lat: number) => void
}

@observer
export default class MapContainer extends React.Component<MapContainerProps, {}> {
    @observable
    private accessor _map: Map | undefined;

    @observable 
    private accessor _marker: Marker | undefined;

    private readonly _mapContainer: React.RefObject<HTMLDivElement> = React.createRef();

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

        if (this.props.onClick && this._map) {
            this._map.on('click', e => {
                if (this._marker) {
                    this._marker.remove()
                }

                this._marker = new MapLibreGL.Marker()
                    .setLngLat(e.lngLat)
                    .addTo(this._map!);

                this.props.onClick!(e.lngLat.lng, e.lngLat.lat)
            })
        }
    }

    render() {
        return <div style={{ width: '100%', height: '33vh' }}>
            <div style={{ width: '100%', height: '100%' }} ref={this._mapContainer} />
        </div>
    }
}