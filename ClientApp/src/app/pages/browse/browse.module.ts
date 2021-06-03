import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {AppRoutingModule} from "../../app-routing.module";
import {WidgetsModule} from "../../widgets/widgets.module";
import { BrowseRestaurantsComponent } from './browse-restaurants/browse-restaurants.component';
import {RouterModule} from "@angular/router";
import { RestaurantsMenuComponent } from './restaurants-menu/restaurants-menu.component';



@NgModule({
  declarations: [BrowseRestaurantsComponent, RestaurantsMenuComponent],
  imports: [
      RouterModule,
      CommonModule,
      WidgetsModule,
      AppRoutingModule
  ]
})
export class BrowseModule { }
