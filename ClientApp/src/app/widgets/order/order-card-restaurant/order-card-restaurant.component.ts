import {Component, Input, OnInit} from '@angular/core';
import {Order} from "../../../models/order/order";

@Component({
  selector: 'app-order-card-restaurant',
  templateUrl: './order-card-restaurant.component.html',
  styleUrls: ['./order-card-restaurant.component.scss']
})
export class OrderCardRestaurantComponent implements OnInit {
    @Input("order") order!: Order;
    @Input("formFactor") formFactor: string = "unspecified";

  constructor() { }

  ngOnInit(): void {
  }

}
