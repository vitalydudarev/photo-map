import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from "../../environments/environment";

@Injectable()
export class YandexDiskService {

  private url: string = `${environment.photoMapApiUrl}/yandex-disk`;

  constructor(private _httpClient: HttpClient) {
  }

  public addUser(id: number, name: string, accessToken: string, expiresIn: number): Observable<any> {
    return this._httpClient.post<any>(`${this.url}`, { id: id, name: name, accessToken: accessToken, tokenExpiresIn: expiresIn });
  }
}
