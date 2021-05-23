import {Component, Input} from '@angular/core';
import {PaymentWrapper} from "../../../models/payment-wrapper";

@Component({
    selector: 'app-payments-table',
    templateUrl: './payments-table.component.html',
    styleUrls: ['./payments-table.component.scss']
})
export class PaymentsTableComponent {
    @Input("showLoading") showLoading!: boolean;
    @Input("payments") items!: PaymentWrapper[];
}
