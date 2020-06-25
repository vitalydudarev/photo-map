import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from "rxjs";

import { GridLayout, Image, PlainGalleryConfig, PlainGalleryStrategy } from '@ks89/angular-modal-gallery';
import { ThumbnailService } from '../services/thumbnail.service';
import { UserService } from '../services/user.service';
import { ImageService } from '../services/image.service';

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
    layout: new GridLayout({ width: '256px', height: 'auto' }, { length: 10, wrap: false })
  };

  constructor(
    private imageService: ImageService,
    private thumbnailService: ThumbnailService,
    private userService: UserService) {
  }

  ngOnInit(): void {
    let i = 0;

    const images: Image[] = [];

    this.subscription = this.userService.getUserImages().subscribe(files => {
      for (let fileName of files) {
        const image = new Image(i, {
            img: this.imageService.getUrl(fileName),
            description: fileName
          },
          {
            img: this.thumbnailService.getUrl(fileName),
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
