import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ManagePickContextComponent} from './manage-pick-context/manage-pick-context.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {ManageIndexComponent} from "./manage-index/manage-index.component";
import { ManageMenuCategoriesComponent } from './manage-menu-categories/manage-menu-categories.component';
import {FormsModule} from "@angular/forms";
import { ManageMenuItemsComponent } from './manage-menu-items/manage-menu-items.component';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {PipesModule} from "../../pipes/pipes.module";
import { ManageOrdersListComponent } from './manage-orders-list/manage-orders-list.component';
import { ManageOrdersDetailsComponent } from './manage-orders-details/manage-orders-details.component';


@NgModule({
    declarations: [ManageIndexComponent, ManagePickContextComponent, ManageMenuCategoriesComponent, ManageMenuItemsComponent, ManageOrdersListComponent, ManageOrdersDetailsComponent],
    imports: [
        CommonModule,
        WidgetsModule,
        FormsModule,
        NgbModule,
        PipesModule,
    ]
})
export class ManageModule {
}
