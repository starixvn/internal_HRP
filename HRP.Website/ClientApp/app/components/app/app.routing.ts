import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from '../app/auth.guard';
import { LoginComponent } from '../login/login.component';
import { TwoFactorAuthenticatorComponent} from '../twofactorauthen/TwoFactorAuthenticator.component';
import { HomeComponent } from '../home/home.component';
import { FetchDataComponent } from '../fetchdata/fetchdata.component';

const appRoutes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'passcode-verify', component: TwoFactorAuthenticatorComponent},
    { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthGuard] },

    // otherwise redirect to home
    { path: '**', redirectTo: 'home' }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);