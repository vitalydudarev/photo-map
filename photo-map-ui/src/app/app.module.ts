import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

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


@NgModule({
  declarations: [
    AppComponent,
    GalleryComponent,
    YandexDiskComponent
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

    GalleryModule.forRoot()
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
export class AppModule { }
