import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, from, Observable, of } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';
import { User } from '../models/user.model';
import { UserService } from '../services/user.service';
import { YandexDiskHubService } from '../services/yandex-disk-hub.service';
import { YandexDiskService } from '../services/yandex-disk.service';
import { ProcessingStatus } from '../models/processing-status.enum';
import { DataService } from '../services/data.service';
import { OAuthConfiguration } from '../models/oauth-configuration.model';
import { OAuthService } from "../services/oauth.service";
import { environment } from "../../environments/environment";

@Component({
  selector: 'app-yandex-disk',
  templateUrl: './yandex-disk.component.html',
  providers: [OAuthConfiguration]
})
export class YandexDiskComponent implements OnInit, OnDestroy {
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
    private userService: UserService,
    private yandexDiskService: YandexDiskService,
    private yandexDiskHubService: YandexDiskHubService,
    private dataService: DataService) {
    this.oAuthService.setConfiguration(environment.oAuth.dropbox as OAuthConfiguration);
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
      switchMap(fragment => {
        if (fragment) {
          const oAuthToken = this.oAuthService.parseAuthResponse(fragment);

          return this.userService.updateUser(this.userId, this.userName, oAuthToken.accessToken, oAuthToken.expiresIn, null, null);
        }

        return of({});
      })
    )
    .subscribe(() => this.router.navigate(['/yandex-disk']));

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


    const sub3 = this.yandexDiskHubService.yandexDiskError().subscribe({
      next: async (error) => {
        this.hasError = true;
        this.error = error;
        this.isRunning = false;
        console.log('processing error.')
        await this.getUser().toPromise();
      }
    });

    const sub4 = this.yandexDiskHubService.yandexDiskProgress().subscribe({
      next: (progress) => {
        this.progressString = `Processed ${progress.processed} of ${progress.total}`;
      }
    });

    this.subscription.add(sub3);
    this.subscription.add(sub4);
  }

  ngOnDestroy(): void {
    this.subscription.add(from(this.yandexDiskHubService.stopHubConnection()).subscribe({
      next: () => console.log('Disconnected from SignalR hub.'),
      error: () => console.log('An error has occurred while disconnecting to SignalR hub.')
    }));

    this.subscription.unsubscribe();
  }

  authorize() {
    this.oAuthService.authorize();
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
      this.status = ProcessingStatus[user.yandexDiskStatus];
      if (user.yandexDiskStatus == ProcessingStatus.Running) {
        this.isRunning = true;
      }
    }));
  }
}
