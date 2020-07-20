import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GalleryComponent } from './gallery/gallery.component';
import { YandexDiskComponent } from './yandex-disk/yandex-disk.component';


const routes: Routes = [
  { path: '', redirectTo: 'gallery', pathMatch: 'full'},
  { path: 'gallery', component: GalleryComponent },
  { path: 'yandex-disk', component: YandexDiskComponent }
  // { path: '', component: AppComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
