import { BrowserModule } from '@angular/platform-browser';
import { NgModule, InjectionToken } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GalleryComponent } from './gallery/gallery.component';

import { MatSidenavModule } from  '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from  '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { HttpClientModule } from '@angular/common/http';
import { GalleryModule } from '@ks89/angular-modal-gallery';

import 'hammerjs'; // Mandatory for angular-modal-gallery 3.x.x or greater (`npm i --save hammerjs`)
import 'mousetrap'; // Mandatory for angular-modal-gallery 3.x.x or greater (`npm i --save mousetrap`)
import { FormsModule } from '@angular/forms';

import { UserService } from './services/user.service';
import { YandexDiskComponent } from './yandex-disk/yandex-disk.component';
import { YandexDiskService } from './services/yandex-disk.service';
import { UserPhotosService } from './services/user-photos.service';
import { SignalRService } from './services/signalr.service';
import { YandexDiskHubService } from './services/yandex-disk-hub.service';
import { DataService } from './services/data.service';
import { OAuthConfiguration } from './models/oauth-configuration.model';
import { environment } from 'src/environments/environment';
import { OAuthService } from './services/oauth.service';
import { OAuthModule } from './oauth.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { MapComponent } from './map/map.component';
import { AgmCoreModule } from '@agm/core';
import {AgmOverlays} from "agm-overlays";
import {AgmMarkerClustererModule} from "@agm/markerclusterer";
import { SharedModule } from './shared/shared.module';

const oAuthConfiguration = new InjectionToken<OAuthConfiguration>(null);

@NgModule({
  declarations: [
    AppComponent,
    GalleryComponent,
    YandexDiskComponent,
    MapComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,

    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,

    GalleryModule.forRoot(),

    SharedModule,

    OAuthModule,

    NgbModule,

    AgmCoreModule.forRoot({
        apiKey: 'AIzaSyDzEycFoLft4yGNSC-F54OUhBMwGr1He_4'
    }),
    AgmOverlays,
    AgmMarkerClustererModule
  ],
  providers: [
    UserService,
    YandexDiskService,
    UserPhotosService,
    YandexDiskHubService,
    DataService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
