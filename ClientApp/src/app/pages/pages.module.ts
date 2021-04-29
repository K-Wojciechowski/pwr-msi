import {NgModule} from '@angular/core';
import {AdminModule} from "./admin/admin.module";
import {AuthModule} from "./auth/auth.module";
import {BaseModule} from "./base/base.module";
import {CommonModule} from '@angular/common';
import {WidgetsModule} from "../widgets/widgets.module";
import {BrowseModule} from "./browse/browse.module";
import {OrdersModule} from "./orders/orders.module";

@NgModule({
    declarations: [],
    exports: [
        AdminModule,
        AuthModule,
        BaseModule,
        BrowseModule,
        OrdersModule
    ],
    imports: [
        CommonModule,
        WidgetsModule
    ]
})
export class PagesModule {
}
