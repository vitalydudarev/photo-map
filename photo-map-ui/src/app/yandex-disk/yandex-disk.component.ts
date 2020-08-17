import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, from } from 'rxjs';
import { map, switchMap, filter, take } from 'rxjs/operators';
import { UserModel } from '../models/user.model';
import { UserService } from '../services/user.service';
import { YandexDiskHubService } from '../services/yandex-disk-hub.service';
import { YandexDiskService } from '../services/yandex-disk.service';

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
  private userId: number = 1;

  constructor(
    private activatedRoute: ActivatedRoute,
    private userService: UserService,
    private yandexDiskService: YandexDiskService,
    private yandexDiskHubService: YandexDiskHubService) {
  }

  async ngOnInit(): Promise<void> {
    const sub1 = this.userService.getUser(this.userId).subscribe((userModel) => {
      this.user = userModel;

      if (Date.now() < new Date(this.user.yandexDiskTokenExpiresOn).getTime()) {
        this.needsAuthorization = false;
        this.tokenExpires = new Date(this.user.yandexDiskTokenExpiresOn).toLocaleString();
      }
    });

    const sub2 = this.activatedRoute.fragment.pipe(
      filter(fragment => fragment !== null && fragment !== ''),
      take(1),
      map(fragment => new URLSearchParams(fragment)),
      map(params => ({
        accessToken: params.get('access_token'),
        expiresIn: params.get('expires_in')
      })),
      switchMap(result => {
        return this.userService.addUser(this.userId, 'user', result.accessToken, parseInt(result.expiresIn));
      })
    )
    .subscribe(() => console.log('done'));

    this.subscription.add(sub1);
    this.subscription.add(sub2);

    this.yandexDiskHubService.buildHubConnection();

    this.subscription.add(from(this.yandexDiskHubService.startHubConnection()).subscribe({
      next: async () => {
        console.log('Connected to SignalR hub.');
        await this.yandexDiskHubService.registerClient(this.userId);
      },
      error: (error) => console.log('An error has occurred while connecting to SignalR hub. ' + error)
    }));
    

    this.yandexDiskHubService.yandexDiskError().subscribe({
      next: () => console.log('yandex disk error')
    });
  }

  ngOnDestroy(): void {
    this.subscription.add(from(this.yandexDiskHubService.stopHubConnection()).subscribe({
      next: () => console.log('Disconnected from SignalR hub.'),
      error: () => console.log('An error has occurred while disconnecting to SignalR hub.')
    }));

    this.subscription.unsubscribe();
  }

  startProcessing() {
    this.yandexDiskService.startProcessing(this.userId).subscribe({
      next: () => console.log('started processing')
    });
  }
}
