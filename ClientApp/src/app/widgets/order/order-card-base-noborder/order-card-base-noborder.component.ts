import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-order-card-base-noborder',
    templateUrl: './order-card-base-noborder.component.html',
    styleUrls: ['./order-card-base-noborder.component.scss']
})
export class OrderCardBaseNoborderComponent implements OnInit {
    @Input("cardTitle") cardTitle!: string;
    @Input("hideTitle") hideTitle: boolean = false;
    @Input("formFactor") formFactor: string = "unspecified";
    @Input("isImportant") isImportant: boolean = true;

    constructor() {
    }

    ngOnInit(): void {
    }

}
