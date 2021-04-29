import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {WidgetsModule} from "../../widgets/widgets.module";
import {AppRoutingModule} from "../../app-routing.module";
import { OrdersOverviewComponent } from './orders-overview/orders-overview.component';
import { PaymentsOverviewComponent } from './payments-overview/payments-overview.component';
import { PaymentsInfoComponent } from './payments-info/payments-info.component';
import { OrdersInfoComponent } from './orders-info/orders-info.component';
import { PaymentsTableComponent } from './payments-table/payments-table.component';
import { PaymentsMakeComponent } from './payments-make/payments-make.component';
import { PaymentsCheckComponent } from './payments-check/payments-check.component';



@NgModule({
  declarations: [OrdersOverviewComponent, PaymentsOverviewComponent, PaymentsInfoComponent, OrdersInfoComponent, PaymentsTableComponent, PaymentsMakeComponent, PaymentsCheckComponent],
  imports: [
    CommonModule,
      WidgetsModule,
      AppRoutingModule
  ]
})
export class OrdersModule { }
