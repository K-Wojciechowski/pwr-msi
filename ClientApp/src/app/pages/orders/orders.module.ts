import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {WidgetsModule} from "../../widgets/widgets.module";
import {AppRoutingModule} from "../../app-routing.module";
import { OrdersOverviewComponent } from './orders-overview/orders-overview.component';
import { PaymentsOverviewComponent } from './payments-overview/payments-overview.component';
import { PaymentsInfoComponent } from './payments-info/payments-info.component';
import { OrdersDetailsComponent } from './orders-details/orders-details.component';
import { PaymentsTableComponent } from './payments-table/payments-table.component';
import { PaymentsMakeComponent } from './payments-make/payments-make.component';
import { PaymentsCheckComponent } from './payments-check/payments-check.component';
import { OrdersMakePaymentComponent } from './orders-make-payment/orders-make-payment.component';



@NgModule({
  declarations: [OrdersOverviewComponent, PaymentsOverviewComponent, PaymentsInfoComponent, OrdersDetailsComponent, PaymentsTableComponent, PaymentsMakeComponent, PaymentsCheckComponent, OrdersMakePaymentComponent],
  imports: [
    CommonModule,
      WidgetsModule,
      AppRoutingModule
  ]
})
export class OrdersModule { }
