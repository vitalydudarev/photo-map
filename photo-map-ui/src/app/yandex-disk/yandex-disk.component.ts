import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, from, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { User } from '../models/user.model';
import { UserService } from '../services/user.service';
import { YandexDiskHubService } from '../services/yandex-disk-hub.service';
import { YandexDiskService } from '../services/yandex-disk.service';
import { YandexDiskStatus } from '../models/yandex-disk-status.enum';
import { DataService } from '../services/data.service';

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
  status: string = '';
  hasError: boolean = false;
  error: string = '';
  isRunning: boolean = false;

  private subscription: Subscription = new Subscription();
  private user: User;
  private userId: number = 1;
  private userName: string = 'user';

  constructor(
    private activatedRoute: ActivatedRoute,
    private userService: UserService,
    private yandexDiskService: YandexDiskService,
    private yandexDiskHubService: YandexDiskHubService,
    private dataService: DataService) {
  }

  async ngOnInit(): Promise<void> {
    const sub1 = this.getUser().subscribe({
      next: () => {
        if (Date.now() < new Date(this.user.yandexDiskTokenExpiresOn).getTime()) {
          this.needsAuthorization = false;
          this.tokenExpires = new Date(this.user.yandexDiskTokenExpiresOn).toLocaleString();
        }
      }
    });

    const sub2 = this.activatedRoute.fragment.pipe(
      map(fragment => {
        if (fragment) {
          const params = new URLSearchParams(fragment);
          const accessToken = params.get('access_token')
          const expiresIn = params.get('expires_in');

          return this.userService.addUser(this.userId, this.userName, accessToken, parseInt(expiresIn));
        }
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
      next: async (error) => {
        this.hasError = true;
        this.error = error;
        this.isRunning = false;
        console.log('processing error.')
        await this.getUser().toPromise();
      }
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
      next: async () => {
        console.log('started processing');
        this.hasError = false;
        this.error = '';
        this.isRunning = true;
        await this.getUser().toPromise();
      }
    });
  }

  stopProcessing() {
    this.yandexDiskService.stopProcessing(this.userId).subscribe({
      next: async () => {
        console.log('stopped processing');
        this.hasError = false;
        this.error = '';
        this.isRunning = false;
        await this.getUser().toPromise();
      }
    });
  }

  deleteAllData() {
    this.dataService.deleteAllData().subscribe({
      next: () => console.log('data deleted')
    })
  }

  private getUser(): Observable<User> {
    return this.userService.getUser(this.userId).pipe(tap((user: User) => {
      this.user = user;
      this.status = YandexDiskStatus[user.yandexDiskStatus];
      if (user.yandexDiskStatus == YandexDiskStatus.Running) {
        this.isRunning = true;
      }
    }));
  }
}
