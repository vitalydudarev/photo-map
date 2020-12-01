import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GalleryComponent } from './modules/gallery/gallery.component';
import { YandexDiskComponent } from './modules/yandex-disk/yandex-disk.component';
import { MapComponent } from './modules/map/map.component';
import { DropboxComponent } from "./modules/dropbox/dropbox.component";


const routes: Routes = [
  { path: '', redirectTo: 'gallery', pathMatch: 'full'},
  { path: 'gallery', component: GalleryComponent },
  { path: 'yandex-disk', component: YandexDiskComponent },
  { path: 'dropbox', component: DropboxComponent },
  { path: 'map', component: MapComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
