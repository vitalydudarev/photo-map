import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Progress } from '../models/progress.model';
import { SignalRService } from './signalr.service';

@Injectable()
export class DropboxHubService extends SignalRService {
    protected hubEvents: string[];
    protected hubUrl = environment.dropboxHub;

    private subjects: { [eventName: string]: Subject<any> };

    constructor() {
        super();
        this.hubEvents = ['DropboxError', 'DropboxProgress'];
        this.createSubjects();
    }

    registerClient(userId: number): Promise<any> {
        return this.hubConnection.invoke('RegisterClient', userId);
    }

    dropboxError(): Observable<string> {
        return this.subjects['DropboxError'].asObservable();
    }

    dropboxProgress(): Observable<Progress> {
        return this.subjects['DropboxProgress'].asObservable();
    }

    buildHubConnection() {
        super.buildHubConnection(this.hubUrl);
    }

    private DropboxError(errorMessage: string) {
        this.subjects['DropboxError'].next(errorMessage);
    }

    private DropboxProgress(progress: any) {
        this.subjects['DropboxProgress'].next(progress);
    }

    private createSubjects() {
        this.subjects = {};
        this.hubEvents.forEach(eventName => {
            this.subjects[eventName] = new Subject<any>();
        });
    }
}
