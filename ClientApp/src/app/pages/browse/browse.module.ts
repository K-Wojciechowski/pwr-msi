import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RestaurantMenuComponent } from './restaurant-menu/restaurant-menu.component';
import {PipesModule} from "../../pipes/pipes.module";
import {WidgetsModule} from "../../widgets/widgets.module";
import {NgbCollapseModule, NgbModalModule} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule} from "@angular/forms";
import {CrystalLightboxModule} from '@crystalui/angular-lightbox';
import {RouterModule} from "@angular/router";

@NgModule({
  declarations: [RestaurantMenuComponent],
    imports: [
        CommonModule,
        PipesModule,
        WidgetsModule,
        NgbCollapseModule,
        NgbModalModule,
        FormsModule,
        CrystalLightboxModule,
        RouterModule
    ]
})
export class BrowseModule { }
