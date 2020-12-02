import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GalleryComponent } from './modules/gallery/gallery.component';

import { MatSidenavModule } from  '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from  '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonToggleModule } from '@angular/material/button-toggle';

import { HttpClientModule } from '@angular/common/http';
import { GalleryModule } from '@ks89/angular-modal-gallery';

import 'hammerjs'; // Mandatory for angular-modal-gallery 3.x.x or greater (`npm i --save hammerjs`)
import 'mousetrap'; // Mandatory for angular-modal-gallery 3.x.x or greater (`npm i --save mousetrap`)
import { FormsModule } from '@angular/forms';

import { UserService } from './core/services/user.service';
import { YandexDiskComponent } from './modules/yandex-disk/yandex-disk.component';
import { YandexDiskService } from './core/services/yandex-disk.service';
import { UserPhotosService } from './core/services/user-photos.service';
import { YandexDiskHubService } from './core/services/yandex-disk-hub.service';
import { DataService } from './core/services/data.service';
import { OAuthModule } from './oauth.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { MapComponent } from './modules/map/map.component';
import { SharedModule } from './modules/shared/shared.module';
import { LocalStorageModule } from "angular-2-local-storage";
import { DropboxComponent } from "./modules/dropbox/dropbox.component";

@NgModule({
  declarations: [
    AppComponent,
    GalleryComponent,
    YandexDiskComponent,
    DropboxComponent,
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

    LocalStorageModule.forRoot({
      prefix: 'dropbox',
      storageType: 'localStorage'
    }),
    MatSnackBarModule,
    MatProgressBarModule,
    MatButtonToggleModule
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
