import { NgModule } from '@angular/core';
import { OAuthServiceFactory } from "./services/oauth-service.factory";


@NgModule({
    imports: [
    ],
    providers: [
        OAuthServiceFactory
    ]
})
export class OAuthModule {
}
