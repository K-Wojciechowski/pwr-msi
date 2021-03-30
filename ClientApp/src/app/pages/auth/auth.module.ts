import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LogInComponent } from './log-in/log-in.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {FormsModule} from "@angular/forms";



@NgModule({
  declarations: [LogInComponent],
    imports: [
        CommonModule,
        FormsModule,
        WidgetsModule
    ]
})
export class AuthModule { }
