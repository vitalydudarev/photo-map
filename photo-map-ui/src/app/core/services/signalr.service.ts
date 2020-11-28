import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

export abstract class SignalRService {
    hubConnection: HubConnection;

    protected abstract hubEvents: string[];
    protected abstract hubUrl: string;

    startHubConnection(): Promise<void> {
        if (this.hubConnection) {
            return this.hubConnection.start();
        }
    }

    stopHubConnection(): Promise<any> {
        if (this.hubConnection) {
            return this.hubConnection.stop();
        }
    }
    
    protected buildHubConnection(hubUrl: string): void {
        this.hubUrl = hubUrl;
        this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl).build();
        this.subscribeToEvents();
    }

    private subscribeToEvents() {
        if (!this.hubEvents) { return; }

        this.hubEvents.forEach(method => {
            this.hubConnection.on(method, (args: any[]) => { this[method](args); });
        });
    }
}
