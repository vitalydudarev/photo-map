import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from "../../environments/environment";
import { User } from '../models/user.model';

@Injectable()
export class UserService {

  private url: string = `${environment.photoMapApiUrl}/users`;

  constructor(private _httpClient: HttpClient) {
  }

  public addUser(id: number, name: string, accessToken: string, expiresIn: number): Observable<any> {
    return this._httpClient.post<any>(`${this.url}`, { id: id, name: name, yandexDiskAccessToken: accessToken, yandexDiskTokenExpiresIn: expiresIn });
  }

  public getUser(id: number): Observable<User> {
    return this._httpClient.get<User>(`${this.url}/${id}`);
  }

  public getUserImages(): Observable<string[]> {
    return  this._httpClient.get<string[]>(`${this.url}/images`);
  }
}
