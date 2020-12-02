import { Component, Input } from '@angular/core';
import { AgmInfoWindow } from '@agm/core';

import { MarkerWrapper } from 'src/app/core/models/marker-wrapper.model';
import { Photo } from 'src/app/core/models/photo.model';

@Component({
    selector: 'app-photos-map-view',
    templateUrl: './photos-map-view.component.html',
    styleUrls: ['./photos-map-view.component.scss']
})
export class PhotosMapViewComponent {
    @Input() photos: Photo[];

    get markers(): MarkerWrapper[] {
        return this.getMarkers();
    }

    center: { lat: number, lng: number } = { lat: 0, lng: 0 };

    private infoWindowOpened: AgmInfoWindow;
    private previousInfoWindow: AgmInfoWindow;

    constructor() {
    }

    onMarkerClicked(infoWindow: AgmInfoWindow) {
        if (!this.previousInfoWindow) {
            this.previousInfoWindow = infoWindow;
        } else {
            this.infoWindowOpened = infoWindow;
            this.previousInfoWindow.close();
        }

        this.previousInfoWindow = infoWindow;
    }

    onMapClicked($event: MouseEvent) {
        if (this.previousInfoWindow) {
            this.previousInfoWindow.close();
        }
    }

    private getMarkers(): MarkerWrapper[] {
        const markers: MarkerWrapper[] = [];

        for (let photo of this.photos) {
            if (photo.latitude && photo.longitude) {
                const title = photo.fileName;
                const marker = this.createMarker(title, photo.latitude, photo.longitude, photo.thumbnailSmallUrl);

                markers.push(marker);
            }
        }

        this.setMapCenter();

        return markers;
    }

    private setMapCenter() {
        const coords = this.calculateMapCenter();

        this.center = {
            lat: coords.lat,
            lng: coords.lng,
        }
    }

    private calculateMapCenter() {
        let minLat: number = this.markers[0].latitude;
        let minLng: number = this.markers[0].longitude;
        let maxLat: number = this.markers[0].latitude;
        let maxLng: number = this.markers[0].longitude;

        for (let i = 1; i < this.markers.length; i++) {
            const marker = this.markers[i];

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

    private createMarker(title: string, latitude: number, longitude: number, thumbnailUrl: string): MarkerWrapper {
        const icon = {
            url: thumbnailUrl,
            scaledSize: { width: 64, height: 64 },
            origin: { x: 0, y: 0 },
            anchor: { x: 0, y: 0 }
        };

        return {
            latitude: latitude,
            longitude: longitude,
            title: title,
            icon: icon,
            previewImageUrl: thumbnailUrl
        } as MarkerWrapper;
    }
}
