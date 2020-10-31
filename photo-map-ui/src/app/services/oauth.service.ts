import { OAuthConfiguration } from '../models/oauth-configuration.model';
import { OAuthToken } from '../models/oauth-token.model';
import { LocalStorageService } from "angular-2-local-storage";
import * as CryptoJS from 'crypto-js';
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import {Injectable} from "@angular/core";

@Injectable()
export class OAuthService {

  private oAuthConfiguration: OAuthConfiguration;

  constructor(
    private storage: LocalStorageService,
    private httpClient: HttpClient) {
  }

  setConfiguration(oAuthConfiguration: OAuthConfiguration) {
    this.oAuthConfiguration = oAuthConfiguration;
  }

  authorize(): void {
    const uri = this.oAuthConfiguration.authorizeUrl;
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

  goToLoginPage() {
    const state = this.strRandom(40);
    const codeVerifier = this.strRandom(128);
    this.storage.set('state', state);
    this.storage.set('codeVerifier', codeVerifier);
    const codeVerifierHash = CryptoJS.SHA256(codeVerifier).toString(CryptoJS.enc.Base64);
    const codeChallenge = codeVerifierHash
      .replace(/=/g, '')
      .replace(/\+/g, '-')
      .replace(/\//g, '_');
    const params = [
      'response_type=' + this.oAuthConfiguration.responseType,
      'state=' + state,
      'client_id=' + this.oAuthConfiguration.clientId,
      'scope=account_info.read files.metadata.read files.content.read',
      'code_challenge=' + codeChallenge,
      'code_challenge_method=S256',
      'redirect_uri=' + encodeURIComponent(this.oAuthConfiguration.redirectUri),
    ];
    window.location.href = this.oAuthConfiguration.authorizeUrl + '?' + params.join('&');
  }

  getAccessToken(code: string, state: string): Observable<any> {
    if (state !== this.storage.get('state')) {
      alert('Invalid state');
      return;
    }
    const payload = new HttpParams()
      .append('grant_type', 'authorization_code')
      .append('code', code)
      .append('code_verifier', this.storage.get('codeVerifier'))
      .append('redirect_uri', this.oAuthConfiguration.redirectUri)
      .append('client_id', this.oAuthConfiguration.clientId);

    return this.httpClient.post<any>(this.oAuthConfiguration.tokenUrl, payload, {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      }
    });
  }

  private strRandom(length: number) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
  }
}
