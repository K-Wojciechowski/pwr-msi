import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersListComponent } from './orders-list/orders-list.component';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { RestaurantsOrderListComponent } from './restaurants-order-list/restaurants-order-list.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {FormsModule} from "@angular/forms";
import {AppRoutingModule} from "../../app-routing.module";



@NgModule({
  declarations: [OrdersListComponent, OrderDetailsComponent, RestaurantsOrderListComponent],
    imports: [
        CommonModule,
        WidgetsModule,
        FormsModule,
        AppRoutingModule
    ]
})
export class DeliveryModule { }
