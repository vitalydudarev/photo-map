import { ModuleWithProviders, NgModule } from '@angular/core';
import { OAuthService } from './services/oauth.service';
import { OAuthConfiguration } from './models/oauth-configuration.model';


@NgModule({
    imports: [
    ],
    providers: [
        OAuthService
    ]
})
export class OAuthModule {

    constructor() {
    }

    static forRoot(oAuthConfiguration: OAuthConfiguration): ModuleWithProviders<OAuthModule> {
        return {
            ngModule: OAuthModule,
            providers: [
                {
                    provide: 'oAuthConfiguration',
                    useValue: oAuthConfiguration,
                    deps: []
                }
            ]
        };
    }
}
