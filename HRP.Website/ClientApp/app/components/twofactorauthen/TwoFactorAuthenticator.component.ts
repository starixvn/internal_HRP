import { Component, OnInit, Inject } from '@angular/core';
import { Http } from '@angular/http';

import { AppConfig } from '../app/app.config';
import { BaseService } from '../../services/base.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ErrorService } from '../../services/error.service';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
    templateUrl: './TwoFactorAuthenticator.component.html',
    providers: [AuthenticationService, BaseService, ErrorService]
})

export class TwoFactorAuthenticatorComponent implements OnInit {
    model: any = {};

    constructor(private baseService: BaseService,
        private errorService: ErrorService,
        private config: AppConfig,
        private router: Router,
        private authenticationService: AuthenticationService) { }

    ngOnInit() {
    }

    verify() {
        console.log(this.model.verifyCode);
        this.authenticationService.twoFactorVerify(this.model.verifyCode)
            .subscribe(data => { this.router.navigate(['/passcode-verify']);}, error => { console.log("Error") });
    }

    
}
