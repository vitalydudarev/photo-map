import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { PhotosMapViewComponent } from './photos-map-view/photos-map-view.component';
import { AgmCoreModule } from '@agm/core';
import { AgmOverlays } from 'agm-overlays';
import { AgmMarkerClustererModule } from '@agm/markerclusterer';
import { GalleryModule } from '@ks89/angular-modal-gallery';
import { PhotosThumbViewComponent } from './photos-thumb-view/photos-thumb-view.component';

import 'hammerjs'; // Mandatory for angular-modal-gallery 3.x.x or greater (`npm i --save hammerjs`)
import 'mousetrap'; // Mandatory for angular-modal-gallery 3.x.x or greater (`npm i --save mousetrap`)
import { ScrollControlComponent } from './scroll-control/scroll-control.component';


@NgModule({
  declarations: [
    ScrollControlComponent,
    PhotosMapViewComponent,
    PhotosThumbViewComponent
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
    GalleryModule.forRoot(),
  ],
  providers: [
  ],
  exports: [
    ScrollControlComponent,
    PhotosMapViewComponent,
    PhotosThumbViewComponent
  ]
})
export class SharedModule { }
