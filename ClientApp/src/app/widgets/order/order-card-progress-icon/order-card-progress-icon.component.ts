import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-order-card-progress-icon',
    templateUrl: './order-card-progress-icon.component.html',
    styleUrls: ['./order-card-progress-icon.component.scss']
})
export class OrderCardProgressIconComponent implements OnInit {
    @Input("icon") icon!: string;
    @Input("text") text!: string;
    @Input("highlighted") highlighted!: boolean;
    @Input("showLine") showLine: boolean = true;

    constructor() {
    }

    ngOnInit(): void {
    }

}
