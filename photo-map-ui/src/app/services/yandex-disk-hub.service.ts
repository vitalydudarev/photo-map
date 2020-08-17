import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { SignalRService } from './signalr.service';

@Injectable()
export class YandexDiskHubService extends SignalRService {
    protected hubEvents: string[];
    protected hubUrl = environment.yandexDiskHub;

    private subjects: { [eventName: string]: Subject<any> };

    constructor() {
        super();
        this.hubEvents = ['YandexDiskError'];
        this.createSubjects();
    }

    registerClient(userId: number): Promise<any> {
        return this.hubConnection.invoke('RegisterClient', userId);
    }

    yandexDiskError(): Observable<string> {
        return this.subjects['YandexDiskError'].asObservable();
    }

    buildHubConnection() {
        super.buildHubConnection(this.hubUrl);
    }

    private YandexDiskError(errorMessage: string) {
        this.subjects['YandexDiskError'].next(errorMessage);
    }

    private createSubjects() {
        this.subjects = {};
        this.hubEvents.forEach(eventName => {
            this.subjects[eventName] = new Subject<any>();
        });
    }
}
