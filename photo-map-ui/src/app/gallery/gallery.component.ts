import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from "rxjs";
import { ImageService } from "../services/image.service";

import { GridLayout, Image, PlainGalleryConfig, PlainGalleryStrategy } from '@ks89/angular-modal-gallery';

@Component({
  selector: 'app-modal-gallery-page',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss']
})
export class GalleryComponent implements OnInit, OnDestroy {

  images: Image[] = [];
  private subscription: Subscription;

  plainGalleryGridConfig: PlainGalleryConfig = {
    strategy: PlainGalleryStrategy.GRID,
    layout: new GridLayout({ width: '128px', height: 'auto' }, { length: 10, wrap: false })
  };

  constructor(private imageService: ImageService) {
  }

  ngOnInit(): void {
    let i = 0;

    const images: Image[] = [];

    this.subscription = this.imageService.getFileKeys().subscribe(files => {
      for (let fileName of files) {
        const image = new Image(i, {
            img: this.imageService.getImageUrl(fileName),
            description: fileName
          },
          {
            img: this.imageService.getThumbUrl(fileName),
            description: fileName
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
