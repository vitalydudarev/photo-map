import { NgModule } from '@angular/core';
import { OAuthService } from "./core/services/oauth.service";


@NgModule({
    imports: [
    ],
    providers: [
        OAuthService
    ]
})
export class OAuthModule {
}
