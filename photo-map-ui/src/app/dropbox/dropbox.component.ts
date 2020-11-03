import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, from, Observable, of } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';
import { User } from '../models/user.model';
import { UserService } from '../services/user.service';
import { OAuthConfiguration } from '../models/oauth-configuration.model';
import { OAuthService } from "../services/oauth.service";
import { environment } from "../../environments/environment";
import { ProcessingStatus } from '../models/processing-status.enum';

@Component({
  selector: 'app-dropbox',
  templateUrl: './dropbox.component.html',
  providers: [OAuthConfiguration]
})
export class DropboxComponent implements OnInit, OnDestroy {
  needsAuthorization: boolean = true;
  tokenExpires: string;
  status: string = '';
  hasError: boolean = false;
  error: string = '';
  isRunning: boolean = false;
  progressString: string = '';

  private subscription: Subscription = new Subscription();
  private user: User;
  private userId: number = 1;
  private userName: string = 'user';

  constructor(
    private router: Router,
    private oAuthService: OAuthService,
    private activatedRoute: ActivatedRoute,
    private userService: UserService) {
    this.oAuthService.setConfiguration(environment.oAuth.dropbox as OAuthConfiguration);
  }

  ngOnInit(): void {

    const sub2 = this.userService.getUser(this.userId).subscribe({
      next: user => {
        if (Date.now() < new Date(user.dropboxTokenExpiresOn).getTime()) {
          this.needsAuthorization = false;
          this.tokenExpires = new Date(user.dropboxTokenExpiresOn).toLocaleString();
          this.status = ProcessingStatus[user.dropboxStatus];
          if (user.dropboxStatus == ProcessingStatus.Running) {
            this.isRunning = true;
          }
        }
      }
    });

    this.subscription.add(sub2);

    const obs1 = this.activatedRoute.queryParams.pipe(switchMap(params => {
      if (params.code) {
        return this.oAuthService.getAccessToken(params.code, params.state);
      }
      else {
        return of(null);
      }
    }));

    const sub1 = obs1.pipe(switchMap(response => {
      if (response) {
        console.log(response);
        const token = response.access_token;
        const expiresIn = response.expires_in ? response.expires_in : null;

        return this.userService.updateUser(this.userId, this.userName, null, null, token, expiresIn);
      }
      else {
        return of(null);
      }
    })).subscribe({
      next: () => {
        console.log('dropbox authorized');
        this.router.navigate(['/dropbox']);
      }
    });

    this.subscription.add(sub1);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  goToLoginPage() {
    this.oAuthService.goToLoginPage();
  }
}
