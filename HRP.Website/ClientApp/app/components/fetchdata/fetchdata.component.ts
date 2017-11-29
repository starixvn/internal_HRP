import { Component, OnInit } from '@angular/core';

import { AppConfig } from '../app/app.config';
import { BaseService } from '../../services/base.service';
import { ErrorService } from '../../services/error.service';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})

export class FetchDataComponent implements OnInit {
    forecasts: WeatherForecast[];

    constructor(private baseService: BaseService,
        private errorService: ErrorService,
        private config: AppConfig) { }

    ngOnInit() {
        this.getWeatherForecasts();
    }

    private getWeatherForecasts() {
        let url = this.config.apiUrl + '/api/SampleData/WeatherForecasts';
        this.baseService.get(url)
            .subscribe(data => { this.forecasts = data as WeatherForecast[]; }, error => { this.errorService.handleError(error); });
    }
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}