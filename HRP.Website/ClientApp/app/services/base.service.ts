import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Injectable()
export class BaseService {
    constructor(private _http: Http) { }

    get(url: string): Observable<any> {
        return this._http.get(url, this.createRequestOptions())
            .map((response: Response) => <any>response.json())
            .catch(this.handleError);
    }

    post(url: string, model: any): Observable<any> {
        let body = JSON.stringify(model);
        return this._http.post(url, body, this.createRequestOptions())
            .map((response: Response) => <any>response.json())
            .catch(this.handleError);
    }

    put(url: string, id: number, model: any): Observable<any> {
        let body = JSON.stringify(model);
        return this._http.put(url + id, body, this.createRequestOptions())
            .map((response: Response) => <any>response.json())
            .catch(this.handleError);
    }

    delete(url: string, id: number): Observable<any> {
        return this._http.delete(url + id, this.createRequestOptions())
            .map((response: Response) => <any>response.json())
            .catch(this.handleError);
    }

    // private helper methods
    private createRequestOptions() {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        return new RequestOptions({ headers: headers });
    }

    private handleError(error: Response) {
        return Observable.throw(error);
    }
}