import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, from, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { User } from '../core/models/user.model';
import { UserService } from '../core/services/user.service';
import { YandexDiskHubService } from '../core/services/yandex-disk-hub.service';
import { YandexDiskService } from '../core/services/yandex-disk.service';
import { ProcessingStatus } from '../core/models/processing-status.enum';
import { DataService } from '../core/services/data.service';
import { OAuthConfiguration } from '../core/models/oauth-configuration.model';
import { OAuthService } from "../core/services/oauth.service";
import { environment } from "../../environments/environment";
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-yandex-disk',
  templateUrl: './yandex-disk.component.html',
  styleUrls: [ './yandex-disk.component.scss' ],
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
  progressBarValue: number = 0;
  progressBarMode: string;

  get action(): string {
    return this.isRunning ? 'Stop' : 'Start';
  }

  private subscriptions: Subscription = new Subscription();
  private user: User;
  private userId: number = 1;
  private userName: string = 'user';

  constructor(
    private router: Router,
    private snackBar: MatSnackBar,
    private oAuthService: OAuthService,
    private activatedRoute: ActivatedRoute,
    private userService: UserService,
    private yandexDiskService: YandexDiskService,
    private yandexDiskHubService: YandexDiskHubService,
    private dataService: DataService) {
      this.oAuthService.setConfiguration(environment.oAuth.yandexDisk as OAuthConfiguration);
  }

  ngOnInit(): void {
    this.getUserData();

    this.onRouteChanged();

    this.startHub();
    this.subscribeToErrorEvent();
    this.subscribeToProgressEvent();
  }

  ngOnDestroy(): void {
    this.subscriptions.add(from(this.yandexDiskHubService.stopHubConnection()).subscribe({
      next: () => this.showSnackBar('Disconnected from SignalR hub.'),
      error: () => this.showSnackBar('An error has occurred while disconnecting to SignalR hub.')
    }));

    this.subscriptions.unsubscribe();
  }

  authorize() {
    this.oAuthService.authorize();
  }

  startStopProcessing() {
    if (!this.isRunning) {
      this.progressBarMode = 'buffer';

      const startProcessingSub = this.yandexDiskService.startProcessing(this.userId).subscribe({
        next: () => {
          this.setState(true, false, '');
          this.showSnackBar('Started processing.')
        },
        error: () => this.showSnackBar('Failed to start processing.')
      });
  
      this.subscriptions.add(startProcessingSub);
    } else {
      const stopProcessingSub = this.yandexDiskService.stopProcessing(this.userId).subscribe({
        next: () => {
          this.setState(false, false);
          this.showSnackBar('Stopped processing');
        },
        error: () => this.showSnackBar('Failed to stop processing.')
      });
  
      this.subscriptions.add(stopProcessingSub);
    }
  }

  deleteAllData() {
    const deleteSub = this.dataService.deleteAllData().subscribe({
      next: () => this.showSnackBar('Data deleted.')
    });

    this.subscriptions.add(deleteSub);
  }

  private getUserData(): void {
    const getUserSub = this.userService.getUser(this.userId).subscribe({
      next: user => {
        this.onGetUser(user);

        if (Date.now() < new Date(this.user.yandexDiskTokenExpiresOn).getTime()) {
          this.needsAuthorization = false;
          this.tokenExpires = new Date(this.user.yandexDiskTokenExpiresOn).toLocaleString();
        }
      },
      error: () => this.showSnackBar('An error has occurred while getting user data.')
    });

    this.subscriptions.add(getUserSub);
  }

  private onRouteChanged(): void {
    const routeSub = this.activatedRoute.fragment.pipe(
      switchMap(fragment => {
        if (fragment) {
          const oAuthToken = this.oAuthService.parseAuthResponse(fragment);

          return this.userService.updateUser(this.userId, this.userName, oAuthToken.accessToken, oAuthToken.expiresIn, null, null);
        }

        return of({});
      })
    )
    .subscribe({
      next: () => this.router.navigate(['/yandex-disk']),
      error: () => this.showSnackBar('An error has occurred while parsing URL.')
    });

    this.subscriptions.add(routeSub);
  }

  private startHub() {
    this.yandexDiskHubService.buildHubConnection();

    const startHubConnectionSub = from(this.yandexDiskHubService.startHubConnection()).pipe(
      switchMap(() => {
        return this.yandexDiskHubService.registerClient(this.userId);
      }))
      .subscribe({
        next: () => this.showSnackBar('Connected to SignalR hub. Client registered.'),
        error: () => this.showSnackBar('An error has occurred while connecting to SignalR hub.')
      });

    this.subscriptions.add(startHubConnectionSub);
  }

  private subscribeToErrorEvent() {
    const errorSub = this.yandexDiskHubService.yandexDiskError().pipe(
      switchMap(error => {
        this.setState(false, true, error);

        return this.userService.getUser(this.userId);
      })
    ).subscribe({
      next: user => this.onGetUser(user),
      error: () => this.showSnackBar('error occurred')
    });

    this.subscriptions.add(errorSub);
  }

  private subscribeToProgressEvent() {
    const progressSub = this.yandexDiskHubService.yandexDiskProgress().subscribe({
      next: (progress) => {
        this.progressBarMode = 'determinate';
        this.progressString = `Processed ${progress.processed} of ${progress.total}`;
        this.progressBarValue = (progress.processed / progress.total) * 100;
        
        if (progress.processed == progress.total) {
          this.isRunning = false;
        }
      }
    });

    this.subscriptions.add(progressSub);
  }

  private onGetUser(user: User) {
    this.user = user;
    this.status = ProcessingStatus[user.yandexDiskStatus];
    this.isRunning = user.yandexDiskStatus == ProcessingStatus.Running;
  }

  private setState(isRunning: boolean, hasError: boolean, error?: string): void {
    this.hasError = hasError;
    this.error = error ? error : '';
    this.isRunning = isRunning;
  }

  private showSnackBar(message: string): void {
    this.snackBar.open(message, 'Close', { duration: 1500 });
  }
}
