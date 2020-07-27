import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { map, switchMap, filter } from 'rxjs/operators';
import { UserModel } from '../models/user.model';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-yandex-disk',
  templateUrl: './yandex-disk.component.html'
})
export class YandexDiskComponent implements OnInit, OnDestroy {
  clientId: string = '66de926ff5be4d2da65e5eb64435687b';
  redirectUri: string = 'http://localhost:4200/yandex-disk';
  yandexDiskUri: string = `https://oauth.yandex.ru/authorize?response_type=token&client_id=${this.clientId}&redirect_uri=${this.redirectUri}`;
  needsAuthorization: boolean = true;
  tokenExpires: string;

  private subscription: Subscription = new Subscription();
  private user: UserModel;

  constructor(private activatedRoute: ActivatedRoute, private userService: UserService) {
  }

  ngOnInit(): void {
    const sub1 = this.userService.getUser(1).subscribe((userModel) => {
      this.user = userModel;

      if (Date.now() < new Date(this.user.yandexDiskTokenExpiresOn).getTime()) {
        this.needsAuthorization = false;
        this.tokenExpires = new Date(this.user.yandexDiskTokenExpiresOn).toLocaleString();
      }
    });

    const sub2 = this.activatedRoute.fragment.pipe(
      filter(fragment => fragment !== null && fragment !== ''),
      map(fragment => new URLSearchParams(fragment)),
      map(params => ({
        accessToken: params.get('access_token'),
        expiresIn: params.get('expires_in')
      })),
      switchMap(result => {
        return this.userService.addUser(1, 'user', result.accessToken, parseInt(result.expiresIn));
      })
    )
    .subscribe(() => console.log('done'));

    this.subscription.add(sub1);
    this.subscription.add(sub2);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
