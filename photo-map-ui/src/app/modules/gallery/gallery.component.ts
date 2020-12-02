import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { UserPhotosService } from '../../core/services/user-photos.service';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpParams } from '@angular/common/http';
import { Location } from '@angular/common';
import { Photo } from 'src/app/core/models/photo.model';

@Component({
    selector: 'app-gallery-page',
    templateUrl: './gallery.component.html',
    styleUrls: ['./gallery.component.scss']
})
export class GalleryComponent implements OnInit, OnDestroy {

    photos: Photo[];
    showSpinner: boolean = false;

    totalCount: number = 0;
    pageIndex: number = 0;
    pageSize: number = 100;
    pageSizes: number[] = [100, 250, 500, 1000];

    thumbViewMode: string = 'thumbViewMode';
    mapViewMode: string = 'mapViewMode';
    selectedViewMode: string = this.thumbViewMode;

    private userId: number = 1;
    private subscription: Subscription;
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
        console.log(this.userId);

        this.subscription = this.userPhotosService.getUserPhotos(this.userId, this.pageSize, this.pageSize * this.pageIndex).subscribe({
            next: pagedResponse => {
                this.totalCount = pagedResponse.total;
                this.photos = pagedResponse.values;
                this.showSpinner = false;
            }
        });
    }
}
