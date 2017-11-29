import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { AppConfig } from './app.config';
import { BaseService } from '../../services/base.service';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private router: Router,
        private baseService: BaseService,
        private config: AppConfig) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let url = this.config.apiUrl + '/api/account/IsAuthenticated';
        let isAuthenticated = false;
        this.baseService.get(url).subscribe(data => isAuthenticated = data, error => {  });

        if(isAuthenticated) {
            return true;
        }

        // not logged in so redirect to login page with the return url
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
    }
}