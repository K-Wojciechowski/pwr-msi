import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RestaurantMenuComponent } from './restaurant-menu/restaurant-menu.component';
import {PipesModule} from "../../pipes/pipes.module";
import {WidgetsModule} from "../../widgets/widgets.module";
import {NgbCollapseModule} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule} from "@angular/forms";



@NgModule({
  declarations: [RestaurantMenuComponent],
    imports: [
        CommonModule,
        PipesModule,
        WidgetsModule,
        NgbCollapseModule,
        FormsModule
    ]
})
export class BrowseModule { }
