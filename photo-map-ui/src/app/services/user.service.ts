import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from "../../environments/environment";

@Injectable()
export class UserService {

  private url: string = `${environment.photoMapApiUrl}/users`;

  constructor(private _httpClient: HttpClient) {
  }

  public getUserImages(): Observable<string[]> {
    return  this._httpClient.get<string[]>(`${this.url}/images`);
  }
}
