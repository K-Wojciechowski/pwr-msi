import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PipesModule} from "../../pipes/pipes.module";
import {WidgetsModule} from "../../widgets/widgets.module";
import {NgbCollapseModule, NgbModalModule} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule} from "@angular/forms";
import {CrystalLightboxModule} from '@crystalui/angular-lightbox';
import {RouterModule} from "@angular/router";
import {AppRoutingModule} from "../../app-routing.module";
import {BrowseRestaurantsComponent} from './browse-restaurants/browse-restaurants.component';
import {RestaurantsMenuComponent} from './restaurants-menu/restaurants-menu.component';



@NgModule({
  declarations: [BrowseRestaurantsComponent, RestaurantMenuComponent],
    imports: [
        CommonModule,
        PipesModule,
        WidgetsModule,
        NgbCollapseModule,
        NgbModalModule,
        FormsModule,
        CrystalLightboxModule,
        RouterModule,
        AppRoutingModule
    ]
})
export class BrowseModule { }
