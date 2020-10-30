import { OAuthConfiguration } from "../models/oauth-configuration.model";
import { OAuthService } from "./oauth.service";
import { Injectable } from "@angular/core";

@Injectable()
export class OAuthServiceFactory {
  create(oAuthConfiguration: OAuthConfiguration): OAuthService {
    return new OAuthService(oAuthConfiguration);
  }
}
