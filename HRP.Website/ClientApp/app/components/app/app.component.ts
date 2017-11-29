import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AppConfig } from './app.config';
import { BaseService } from '../../services/base.service';
import { ErrorService } from '../../services/error.service';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
    isAuthenticated: boolean = false;

    constructor(private router: Router,
        private baseService: BaseService,
        private errorService: ErrorService,
        private config: AppConfig) { }

    ngOnInit() {
        let url = this.config.apiUrl + '/api/account/IsAuthenticated';
        this.baseService.get(url)
            .subscribe(data => { 
                this.isAuthenticated = data as boolean;
            }, error => { this.errorService.handleError(error); });
    }
}