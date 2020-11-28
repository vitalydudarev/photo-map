import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from "rxjs";

import { UserPhotosService } from '../../core/services/user-photos.service';
import { environment } from 'src/environments/environment';
import { AgmInfoWindow } from '@agm/core';

export class MarkerWrapper {
  latitude: number;
  longitude: number;
  title: string;
  icon: any;
  previewImageUrl: string;
}

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, OnDestroy {

  markers: any[] = [];
  center: { lat: number, lng: number } = { lat: 0, lng: 0 };

  private apiUrl: string = `${environment.photoMapApiUrl}`;

  private subscription: Subscription;

  private infoWindowOpened: AgmInfoWindow;
  private previousInfoWindow: AgmInfoWindow;

  constructor(private userPhotosService: UserPhotosService) {
  }

  ngOnInit(): void {
    navigator.geolocation.getCurrentPosition((position) => {
      this.center = {
        lat: position.coords.latitude,
        lng: position.coords.longitude,
      }
    });

    this.getPhotos();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
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

  private getPhotos() {
    this.subscription = this.userPhotosService.getUserPhotos(1, 100, 0).subscribe(pageResponse => {
      let i = 1;

      for (let photo of pageResponse.values) {
        if (photo.latitude && photo.longitude) {
          const title = photo.fileName;
          const marker = this.createMarker(title, photo.latitude, photo.longitude, `${this.apiUrl}/${photo.thumbnailUrl}`);

          this.markers.push(marker);

          i++;
        }
      }
    });
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
