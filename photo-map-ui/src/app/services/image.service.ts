import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from "../../environments/environment";

@Injectable()
export class ImageService {

  private url: string = environment.photoMapApiUrl + 'images';

  constructor(private _httpClient: HttpClient) {
  }

  public getFileKeys(): Observable<string[]> {
    return  this._httpClient.get<string[]>(this.url + '/all');
  }

  public getImageUrl(fileName: string) : string {
    return this.url + '/image/' + fileName;
  }

  public getThumbUrl(fileName: string) : string {
    return this.url + '/thumb/' + fileName;
  }
}
