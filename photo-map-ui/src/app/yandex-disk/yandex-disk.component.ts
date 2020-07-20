import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { YandexDiskService } from '../services/yandex-disk.service';

@Component({
  selector: 'app-yandex-disk',
  templateUrl: './yandex-disk.component.html'
})
export class YandexDiskComponent implements OnInit, OnDestroy {
  clientId: string = '66de926ff5be4d2da65e5eb64435687b';
  redirectUri: string = 'http://localhost:4200/yandex-disk';
  yandexDiskUri: string = `https://oauth.yandex.ru/authorize?response_type=token&client_id=${this.clientId}&redirect_uri=${this.redirectUri}`;

  private subscription: Subscription;

  constructor(private activatedRoute: ActivatedRoute, private yandexDiskService: YandexDiskService) {
  }

  ngOnInit(): void {
    this.subscription = this.activatedRoute.fragment.pipe(
      map(fragment => new URLSearchParams(fragment)),
      map(params => ({
        accessToken: params.get('access_token'),
        expiresIn: params.get('expires_in')
      }))
    )
    .pipe(switchMap(result => {
      return this.yandexDiskService.addUser(1, 'user', result.accessToken, parseInt(result.expiresIn));
    }))
    .subscribe(() => console.log('done'));
    /*.subscribe((result) => {
      console.log(result.accessToken);
      console.log(result.expiresIn);
    })*/
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
