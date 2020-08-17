import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from "../../environments/environment";

@Injectable()
export class YandexDiskService {

  private url: string = `${environment.photoMapApiUrl}/yandex-disk`;

  constructor(private _httpClient: HttpClient) {
  }

  startProcessing(userId: number): Observable<any> {
    return this._httpClient.post(this.url, userId);
  }
}
