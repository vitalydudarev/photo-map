import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from "rxjs";

import { GridLayout, Image, PlainGalleryConfig, PlainGalleryStrategy, DotsConfig } from '@ks89/angular-modal-gallery';
import { UserPhotosService } from '../../core/services/user-photos.service';
import { environment } from 'src/environments/environment';
import { ButtonsStrategy, ButtonsConfig } from '@ks89/angular-modal-gallery';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpParams } from '@angular/common/http';
import { Location } from '@angular/common';

@Component({
  selector: 'app-modal-gallery-page',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss']
})
export class GalleryComponent implements OnInit, OnDestroy {

  showSpinner: boolean = false;
  images: Image[] = [];

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
  pageIndex: number = 0;
  pageSize: number = 100;
  pageSizes: number[] = [100, 250, 500, 1000];

  tileViewMode: string = 'tileViewMode';
  mapViewMode: string = 'mapViewMode';
  selectedViewMode: string = this.tileViewMode;

  private subscription: Subscription;
  private apiUrl: string = `${environment.photoMapApiUrl}`;
  private pageConst = 'page';
  private pageSizeConst = 'pageSize';

  constructor(
    private router: Router,
    private location: Location,
    private activatedRoute: ActivatedRoute,
    private userPhotosService: UserPhotosService) {
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe({
      next: params => {
        const pageIndex = params[this.pageConst];
        const pageSize = params[this.pageSizeConst];

        if (pageIndex) {
          this.pageIndex = parseInt(pageIndex) - 1;
        }
        
        if (pageSize) {
          this.pageSize = parseInt(pageSize);
        }
      }
    });

    this.setImages();
    this.addQueryString();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onViewModeChanged(value: string) {
    this.selectedViewMode = value;
  }

  pageUpdated(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;

    this.addQueryString();

    this.setImages();
  }

  private addQueryString() {
    const params = new HttpParams()
      .append(this.pageConst, (this.pageIndex + 1).toString())
      .append(this.pageSizeConst, this.pageSize.toString());

    this.location.go(this.router.url.split("?")[0], params.toString());
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
            img: `${this.apiUrl}/${photo.thumbnailLargeUrl}`,
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
