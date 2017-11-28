import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Router } from '@angular/router';

@Injectable()
export class ErrorService {
    constructor(private router: Router) { }    

    handleError(error: Response) {
        switch (error.status) {
            case 401:
                this.router.navigate(['/login']);
                break;

            case 405:
                break;

            case 500:
                break;
        }
    }
}