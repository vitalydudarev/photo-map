import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from "rxjs";

import { GridLayout, Image, PlainGalleryConfig, PlainGalleryStrategy, DotsConfig } from '@ks89/angular-modal-gallery';
import { UserPhotosService } from '../services/user-photos.service';
import { environment } from 'src/environments/environment';
import { ButtonsStrategy, ButtonsConfig } from '@ks89/angular-modal-gallery';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-modal-gallery-page',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss']
})
export class GalleryComponent implements OnInit, OnDestroy {

  showSpinner: boolean = false;
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
    layout: new GridLayout({ width: '256px', height: 'auto' }, { length: 20, wrap: true }),
  };

  totalCount: number = 0;
  pageIndex: number = 0
  pageSize: number = 100;
  pageSizes: number[] = [100, 250, 500, 1000];

  constructor(
    private userPhotosService: UserPhotosService) {
  }

  ngOnInit(): void {
    this.setImages();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  pageUpdated(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;

    this.setImages();
  }

  private setImages() {

    this.showSpinner = true;

    let i = 0;

    const images: Image[] = [];

    this.subscription = this.userPhotosService.getUserPhotos(1, this.pageSize, this.pageSize * this.pageIndex).subscribe(photos => {
      this.totalCount = photos.total;

      for (let photo of photos.values) {
        const image = new Image(i, {
            img: `${this.apiUrl}/${photo.photoUrl}`,
            description: photo.fileName
          },
          {
            img: `${this.apiUrl}/photos/${photo.thumbnailLargeFileId}`,
            description: photo.fileName
          });

        images.push(image);
        i++;
      }

      this.images = images;
      this.showSpinner = false;
    });
  }
}
