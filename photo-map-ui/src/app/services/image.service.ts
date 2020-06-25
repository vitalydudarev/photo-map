import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";

@Injectable()
export class ImageService {

  private url: string = `${environment.photoMapApiUrl}/images`;

  constructor() {
  }

  public getUrl(fileName: string) : string {
    return `${this.url}/${fileName}`;
  }
}
