import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from './core/services/user.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
    title = 'photo-map-ui';

    menuItems = [
        { title: "Gallery", route: "/gallery" },
        { title: "Yandex.Disk", route: "/yandex-disk" },
        { title: "Dropbox", route: "/dropbox" },
        { title: "Map", route: "/map"}
    ];

    userId = 1;
    yandexDiskAuthorized: boolean;
    dropboxAuthorized: boolean;

    private subscriptions: Subscription = new Subscription();

    constructor(private userService: UserService) {
    }

    ngOnInit(): void {
        this.getUserData();
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    private getUserData(): void {
        const getUserSub = this.userService.getUser(this.userId).subscribe({
            next: user => {
                if (Date.now() < new Date(user.yandexDiskTokenExpiresOn).getTime()) {
                    this.yandexDiskAuthorized = true;
                }

                if (Date.now() < new Date(user.dropboxTokenExpiresOn).getTime()) {
                    this.dropboxAuthorized = true;
              }
            },
            error: () => console.log('An error has occurred while getting user data.')
        });

        this.subscriptions.add(getUserSub);
    }
}
