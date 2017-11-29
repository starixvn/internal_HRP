import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { AppConfig } from '../components/app/app.config';

@Injectable()
export class AuthenticationService {
    constructor(private http: Http, private config: AppConfig, private router: Router) { }

    login(username: string, password: string): Observable<any> {
        let body = JSON.stringify({ username: username, password: password });
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(this.config.apiUrl + '/api/account/login', body, options)
            .map((response: Response) => <any>response)
            .catch(this.handleError);
    }

    twoFactorVerify(verifyCode: string): Observable<any> {
        let body = JSON.stringify({verifyCode: verifyCode});
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(this.config.apiUrl + '/api/account/verify-code', body, options)
        .map((response: Response) => {
            let user = response.json();
            if (user && user.token) {
                localStorage.setItem('currentUser', JSON.stringify(user));
            }
        })
        .catch(this.handleError);
    }

    logout() {
    }

    private handleError(error: Response) {
        return Observable.throw(error);
    }
}