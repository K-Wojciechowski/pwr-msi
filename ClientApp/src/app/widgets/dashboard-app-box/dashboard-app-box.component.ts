import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-dashboard-app-box',
    templateUrl: './dashboard-app-box.component.html',
    styleUrls: ['./dashboard-app-box.component.scss']
})
export class DashboardAppBoxComponent implements OnInit {
    @Input("routerLink") routerLink!: string;
    @Input("background") background!: string;
    @Input("icon") icon!: string;
    @Input("name") name!: string;
    @Input("description") description!: string;

    constructor() {
    }

    ngOnInit(): void {
    }

}
