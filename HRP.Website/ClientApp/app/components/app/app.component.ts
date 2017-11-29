import { Component, OnInit, Inject } from '@angular/core';
import { Http } from '@angular/http';

import { BaseService } from '../../services/base.service';
import { ErrorService } from '../../services/error.service';

import { IMenuItem } from '../../models/menu-item';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
    menus: IMenuItem[];

    constructor(private http: Http,
        private baseService: BaseService,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) { }

    ngOnInit() {
        this.getMenuByCurrentUser();
    }

    private getMenuByCurrentUser() {
        let url = this.baseUrl + 'home/GetMenuByCurrentUser';
        this.baseService.get(url)
            .subscribe(data => { this.menus = data as IMenuItem[]; }, error => { this.errorService.handleError(error); });
    }
}