import { MarkerWrapper } from '../models/marker-wrapper.model';

export class GoogleMapHelper {
    /* https://stackoverflow.com/questions/6048975/google-maps-v3-how-to-calculate-the-zoom-level-for-a-given-bounds */
    static getBoundsZoomLevel(markers: MarkerWrapper[], mapDim): number {
        const WORLD_DIM = { height: 256, width: 256 };
        const ZOOM_MAX = 21;

        const ne = this.getNorthEast(markers);
        const sw = this.getSouthWest(markers);

        const latFraction = (this.latRad(ne.lat) - this.latRad(sw.lat)) / Math.PI;

        const lngDiff = ne.lng - sw.lng;
        const lngFraction = ((lngDiff < 0) ? (lngDiff + 360) : lngDiff) / 360;

        const latZoom = this.zoom(mapDim.height, WORLD_DIM.height, latFraction);
        const lngZoom = this.zoom(mapDim.width, WORLD_DIM.width, lngFraction);

        return Math.min(latZoom, lngZoom, ZOOM_MAX);
    }

    static getCenter(markers: MarkerWrapper[]) {
        let minLat: number = markers[0].latitude;
        let minLng: number = markers[0].longitude;
        let maxLat: number = markers[0].latitude;
        let maxLng: number = markers[0].longitude;

        for (let i = 1; i < markers.length; i++) {
            const marker = markers[i];

            if (marker.latitude < minLat) {
                minLat = marker.latitude;
            }

            if (marker.latitude > maxLat) {
                maxLat = marker.latitude;
            }

            if (marker.longitude < minLng) {
                minLng = marker.longitude;
            }

            if (marker.longitude > maxLng) {
                maxLng = marker.longitude;
            }
        }

        const centerLat = (minLat + maxLat) / 2;
        const centerLng = (minLng + maxLng) / 2;

        return { lat: centerLat, lng: centerLng };
    }

    static calculateCenter(markers: MarkerWrapper[]) {
        let lng = 0;
        let lat = 0;

        for (const marker of markers) {
            lat = lat + marker.latitude;
            lng += marker.longitude;
        }

        return { lat: lat / markers.length, lng: lng / markers.length };
    }

    private static zoom(mapPx: number, worldPx: number, fraction: number): number {
        return Math.floor(Math.log(mapPx / worldPx / fraction) / Math.LN2);
    }

    private static latRad(lat: number): number {
        const sin = Math.sin(lat * Math.PI / 180);
        const radX2 = Math.log((1 + sin) / (1 - sin)) / 2;
        return Math.max(Math.min(radX2, Math.PI), -Math.PI) / 2;
    }

    private static getNorthEast(markers: MarkerWrapper[]) {
        let maxLat: number = markers[0].latitude;
        let maxLng: number = markers[0].longitude;

        for (let i = 1; i < markers.length; i++) {
            const marker = markers[i];

            if (marker.latitude > maxLat) {
                maxLat = marker.latitude;
            }

            if (marker.longitude > maxLng) {
                maxLng = marker.longitude;
            }
        }

        return { lat: maxLat, lng: maxLng };
    }

    private static getSouthWest(markers: MarkerWrapper[]) {
        let minLat: number = markers[0].latitude;
        let minLng: number = markers[0].longitude;

        for (let i = 1; i < markers.length; i++) {
            const marker = markers[i];

            if (marker.latitude < minLat) {
                minLat = marker.latitude;
            }

            if (marker.longitude < minLng) {
                minLng = marker.longitude;
            }
        }

        return { lat: minLat, lng: minLng };
    }
}
