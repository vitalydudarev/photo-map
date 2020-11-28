import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Photo } from '../models/photo.model';
import { PagedResponse } from '../models/paged-response.model';
import { environment } from 'src/environments/environment';

@Injectable()
export class UserPhotosService {

  private url: string = `${environment.photoMapApiUrl}/users`;

  constructor(private _httpClient: HttpClient) {
  }

  public getUserPhotos(userId: number, top: number, skip: number): Observable<PagedResponse<Photo>> {
    return  this._httpClient.get<PagedResponse<Photo>>(`${this.url}/${userId}/photos?top=${top}&skip=${skip}`);
  }
}
