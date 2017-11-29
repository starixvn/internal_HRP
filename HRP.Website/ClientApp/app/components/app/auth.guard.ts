import { Injectable, Inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { BaseService } from '../../services/base.service';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private baseService: BaseService,
        @Inject('BASE_URL') private baseUrl: string) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let url = this.baseUrl + 'home/IsAuthenticated';
        let isAuthenticated = false;
        this.baseService.get(url).subscribe(data => isAuthenticated = data, error => {  });

        if(isAuthenticated) {
            return true;
        }
        
        return false;
    }
}