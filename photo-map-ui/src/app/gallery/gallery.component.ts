import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from "rxjs";

import { GridLayout, Image, PlainGalleryConfig, PlainGalleryStrategy, DotsConfig } from '@ks89/angular-modal-gallery';
import { UserPhotosService } from '../services/user-photos.service';
import { environment } from 'src/environments/environment';
import { ButtonsStrategy, ButtonsConfig } from '@ks89/angular-modal-gallery';

@Component({
  selector: 'app-modal-gallery-page',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss']
})
export class GalleryComponent implements OnInit, OnDestroy {

  images: Image[] = [];
  private subscription: Subscription;
  private apiUrl: string = `${environment.photoMapApiUrl}`;

  dotsConfig: DotsConfig = {
    visible: false
  }

  buttonsConfig: ButtonsConfig = {
    visible: true,
    strategy: ButtonsStrategy.SIMPLE
  };

  plainGalleryGridConfig: PlainGalleryConfig = {
    strategy: PlainGalleryStrategy.GRID,
    layout: new GridLayout({ width: '128px', height: 'auto' }, { length: 20, wrap: true }),
  };

  constructor(
    private userPhotosService: UserPhotosService) {
  }

  ngOnInit(): void {
    let i = 0;

    const images: Image[] = [];

    this.subscription = this.userPhotosService.getUserPhotos(1).subscribe(photos => {
      for (let photo of photos) {
        const image = new Image(i, {
            img: `${this.apiUrl}/${photo.photoUrl}`,
            description: photo.fileName
          },
          {
            img: `${this.apiUrl}/${photo.thumbnailUrl}`,
            description: photo.fileName
          });

        images.push(image);
        i++;
      }

      this.images = images;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
