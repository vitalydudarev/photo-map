import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

import { GridLayout, Image, PlainGalleryConfig, PlainGalleryStrategy, DotsConfig } from '@ks89/angular-modal-gallery';
import { ButtonsStrategy, ButtonsConfig } from '@ks89/angular-modal-gallery';
import { Photo } from 'src/app/core/models/photo.model';

@Component({
    selector: 'app-photos-thumb-view',
    templateUrl: './photos-thumb-view.component.html',
    styleUrls: ['./photos-thumb-view.component.scss']
})
export class PhotosThumbViewComponent implements OnChanges {

    @Input() photos: Photo[];

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

    constructor() {
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.setImages();
    }

    private setImages() {
        let i = 0;

        const images: Image[] = [];

        for (let photo of this.photos) {
            const image = new Image(i, {
                img: photo.photoUrl,
                description: photo.fileName
            },
            {
                img: photo.thumbnailLargeUrl,
                description: photo.fileName
            });

            images.push(image);
            i++;
        }

        this.images = images;
    }
}
