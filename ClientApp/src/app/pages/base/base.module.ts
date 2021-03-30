import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IndexComponent } from './index/index.component';
import { WelcomeNewUsersComponent } from './welcome-new-users/welcome-new-users.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {AppRoutingModule} from "../../app-routing.module";

@NgModule({
    declarations: [IndexComponent, WelcomeNewUsersComponent, DashboardComponent],
    imports: [
        CommonModule,
        WidgetsModule,
        AppRoutingModule
    ]
})
export class BaseModule { }
