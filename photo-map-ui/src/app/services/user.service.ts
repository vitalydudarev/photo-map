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

  addUser(id: number, name: string): Observable<any> {
    return this._httpClient.post<any>(`${this.url}`, { id: id, name: name });
  }

  updateUser(id: number, name: string, yandexDiskToken: string, yandexDiskTokenExpiresIn: number, dropboxToken: string, dropboxTokenExpiresIn: number): Observable<any> {
    return this._httpClient.patch<any>(`${this.url}/${id}`, { name: name, yandexDiskToken: yandexDiskToken, yandexDiskTokenExpiresIn: yandexDiskTokenExpiresIn, dropboxToken: dropboxToken, dropboxTokenExpiresIn: dropboxTokenExpiresIn });
  }

  getUser(id: number): Observable<User> {
    return this._httpClient.get<User>(`${this.url}/${id}`);
  }

  getUserImages(): Observable<string[]> {
    return  this._httpClient.get<string[]>(`${this.url}/images`);
  }
}
