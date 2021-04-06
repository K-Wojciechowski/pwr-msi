import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {UsersListComponent} from './users-list/users-list.component';
import {UsersEditComponent} from './users-edit/users-edit.component';
import {RestaurantsListComponent} from './restaurants-list/restaurants-list.component';
import {RestaurantsEditComponent} from './restaurants-edit/restaurants-edit.component';
import {UsersRestaurantsEditorComponent} from './users-restaurants-editor/users-restaurants-editor.component';
import {UsersAddComponent} from './users-add/users-add.component';
import {UsersEditorComponent} from './users-editor/users-editor.component';
import {RestaurantsAddComponent} from './restaurants-add/restaurants-add.component';
import {RestaurantsEditorComponent} from './restaurants-editor/restaurants-editor.component';
import {RouterModule} from "@angular/router";
import {WidgetsModule} from "../../widgets/widgets.module";

@NgModule({
    declarations: [UsersListComponent, UsersEditComponent, RestaurantsListComponent, RestaurantsEditComponent, UsersRestaurantsEditorComponent, UsersAddComponent, UsersEditorComponent, RestaurantsAddComponent, RestaurantsEditorComponent],
    imports: [
        CommonModule,
        RouterModule,
        WidgetsModule,
    ]
})
export class AdminModule {
}
