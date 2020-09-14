import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";

import { UserPhotosService } from '../services/user-photos.service';
import { MapMarker, MapInfoWindow, GoogleMap } from '@angular/google-maps';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, OnDestroy {
  @ViewChild(MapInfoWindow, { static: false }) infoWindow: MapInfoWindow
  @ViewChild(GoogleMap, { static: false }) map: GoogleMap;

  zoom = 8
  center: google.maps.LatLngLiteral
  options: google.maps.MapOptions = {
    mapTypeId: 'hybrid',
    zoomControl: true,
    scrollwheel: true,
    disableDoubleClickZoom: false,
    maxZoom: 20,
    minZoom: 8,
    gestureHandling: 'greedy'
  }

  currentImageUrl: string;
  markers: any[] = [];

  private apiUrl: string = `${environment.photoMapApiUrl}`;
  private photos: { [id: string] : string; } = {};

  private subscription: Subscription;

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

  openInfo(marker: MapMarker) {
    this.currentImageUrl = this.photos[marker.getTitle()];
    this.infoWindow.open(marker);
  }

  private getPhotos() {
    this.subscription = this.userPhotosService.getUserPhotos(1, 100, 0).subscribe(pageResponse => {
      let i = 1;

      for (let photo of pageResponse.values) {
        const title = photo.fileName;
        const marker = this.createMarker(title, photo.latitude, photo.longitude, `${this.apiUrl}/${photo.thumbnailUrl}`);

        this.photos[title] = `${this.apiUrl}/photos/${photo.thumbnailSmallFileId}`;
        this.markers.push(marker);

        i++;
      }
    });
  }

  private createMarker(title: string, latitude: number, longitude: number, thumbnailUrl: string): MapMarker {
    const image = {
      url: thumbnailUrl,
      scaledSize: new google.maps.Size(64, 64),
      origin: new google.maps.Point(0, 0),
      anchor: new google.maps.Point(0, 0)
    } as google.maps.Icon;

    return {
      position: {
        lat: latitude,
        lng: longitude,
      },
      title: title,
      options: {
        icon: image
      }
    } as MapMarker;
  }
}
