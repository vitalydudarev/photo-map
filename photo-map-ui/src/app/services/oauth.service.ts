import { Injectable, Inject } from '@angular/core';
import { OAuthConfiguration } from '../models/oauth-configuration.model';
import { OAuthToken } from '../models/oauth-token.model';

@Injectable()
export class OAuthService {

  constructor(@Inject('oAuthConfiguration') private oAuthConfiguration: OAuthConfiguration) {
  }

  authorize(): void {
    const uri = this.oAuthConfiguration.uri;
    const responseType = this.oAuthConfiguration.responseType;
    const clientId = this.oAuthConfiguration.clientId;
    const redirectUri = this.oAuthConfiguration.redirectUri;

    const url = `${uri}?response_type=${responseType}&client_id=${clientId}&redirect_uri=${redirectUri}`;

    window.location.href = url;
  }

  parseAuthResponse(queryParams: string): OAuthToken {
    const params = new URLSearchParams(queryParams);
    const accessToken = params.get('access_token')
    const expiresIn = parseInt(params.get('expires_in'));

    return { accessToken: accessToken, expiresIn: expiresIn } as OAuthToken;
  }
}
