import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {AppRoutingModule} from "../../app-routing.module";
import {WidgetsModule} from "../../widgets/widgets.module";
import { BrowseRestaurantsComponent } from './browse-restaurants/browse-restaurants.component';



@NgModule({
  declarations: [BrowseRestaurantsComponent],
  imports: [
      CommonModule,
      WidgetsModule,
      AppRoutingModule
  ]
})
export class BrowseModule { }
