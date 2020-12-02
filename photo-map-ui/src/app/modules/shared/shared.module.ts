import { NgModule } from '@angular/core';
import { ScrollTopComponent } from './scroll-top/scroll-top.component';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { PhotosMapViewComponent } from './photos-map-view/photos-map-view.component';
import { AgmCoreModule } from '@agm/core';
import { AgmOverlays } from 'agm-overlays';
import { AgmMarkerClustererModule } from '@agm/markerclusterer';


@NgModule({
  declarations: [
    ScrollTopComponent,
    PhotosMapViewComponent
  ],
  imports: [
    MatIconModule,
    MatButtonModule,
    CommonModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyDzEycFoLft4yGNSC-F54OUhBMwGr1He_4'
    }),
    AgmOverlays,
    AgmMarkerClustererModule,
  ],
  providers: [
  ],
  exports: [
    ScrollTopComponent,
    PhotosMapViewComponent
  ]
})
export class SharedModule { }
