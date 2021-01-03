import { Component, ElementRef, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { AgmInfoWindow } from '@agm/core';

import { MarkerWrapper } from 'src/app/core/models/marker-wrapper.model';
import { Photo } from 'src/app/core/models/photo.model';
import { GoogleMapHelper } from 'src/app/core/helpers/google-map.helper';

@Component({
    selector: 'app-photos-map-view',
    templateUrl: './photos-map-view.component.html'
})
export class PhotosMapViewComponent implements OnChanges {
    @Input() photos: Photo[];
    @ViewChild('map', {read: ElementRef, static: true}) map: ElementRef;

    markers: MarkerWrapper[] = [];
    zoom: number;
    center: { lat: number, lng: number } = { lat: 0, lng: 0 };

    private infoWindowOpened: AgmInfoWindow;
    private previousInfoWindow: AgmInfoWindow;

    constructor() {
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.previousInfoWindow = null;
        this.infoWindowOpened = null;

        const markers = this.createMarkers();

        const map = document.getElementsByClassName('agm-map-container-inner sebm-google-map-container-inner')[0];

        this.zoom = GoogleMapHelper.getBoundsZoomLevel(markers, { height: map.clientHeight, width: map.clientWidth });
        this.center = GoogleMapHelper.getCenter(markers);
        this.markers = markers;
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

    private createMarkers(): MarkerWrapper[] {
        const markers: MarkerWrapper[] = [];

        for (const photo of this.photos) {
            if (photo.latitude && photo.longitude) {
                const title = photo.fileName;
                const marker = this.createMarker(title, photo.latitude, photo.longitude, photo.thumbnailSmallUrl);

                markers.push(marker);
            }
        }

        return markers;
    }

    private createMarker(title: string, latitude: number, longitude: number, thumbnailUrl: string): MarkerWrapper {
        const icon = {
            url: thumbnailUrl,
            scaledSize: { width: 64, height: 64 },
            origin: { x: 0, y: 0 },
            anchor: { x: 0, y: 0 }
        };

        return {
            latitude,
            longitude,
            title,
            icon,
            previewImageUrl: thumbnailUrl
        } as MarkerWrapper;
    }
}
