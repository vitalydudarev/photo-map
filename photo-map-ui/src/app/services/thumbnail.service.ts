import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";

@Injectable()
export class ThumbnailService {

  private url: string = `${environment.photoMapApiUrl}/thumbs`;

  constructor() {
  }

  public getUrl(fileName: string) : string {
    return `${this.url}/${fileName}`;
  }
}
