import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { UserPhotosService } from '../../core/services/user-photos.service';
import { environment } from 'src/environments/environment';
import { Photo } from 'src/app/core/models/photo.model';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html'
})
export class MapComponent implements OnInit, OnDestroy {

  photos: Photo[];

  private apiUrl: string = `${environment.photoMapApiUrl}`;

  private subscription: Subscription;

  private userId: number = 1;
  private pageSize: number = 100;

  constructor(private userPhotosService: UserPhotosService) {
  }

  ngOnInit(): void {
    this.setMarkers();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private setMarkers() {
    this.subscription = this.userPhotosService.getUserPhotos(this.userId, this.pageSize, 0).subscribe(pagedResponse => {
      this.photos = pagedResponse.values;
    });
  }
}
