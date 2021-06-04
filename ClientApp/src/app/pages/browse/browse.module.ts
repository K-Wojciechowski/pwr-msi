import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {PipesModule} from "../../pipes/pipes.module";
import {WidgetsModule} from "../../widgets/widgets.module";
import {NgbCollapseModule, NgbModalModule} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule} from "@angular/forms";
import {RouterModule} from "@angular/router";

@NgModule({
  declarations: [],
    imports: [
        CommonModule,
        PipesModule,
        WidgetsModule,
        NgbCollapseModule,
        NgbModalModule,
        FormsModule,
        RouterModule
    ]
})
export class BrowseModule { }
