import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from "../../environments/environment";

@Injectable()
export class DataService {

  private url: string = `${environment.photoMapApiUrl}/data`;

  constructor(private _httpClient: HttpClient) {
  }

  deleteAllData(): Observable<any> {
    return this._httpClient.delete(this.url);
  }
}
