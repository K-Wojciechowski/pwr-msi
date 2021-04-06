import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LogInComponent} from './log-in/log-in.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {FormsModule} from "@angular/forms";
import {RegisterComponent} from './register/register.component';
import {VerifyEmailComponent} from './verify-email/verify-email.component';
import {AppRoutingModule} from "../../app-routing.module";
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';


@NgModule({
    declarations: [LogInComponent, RegisterComponent, VerifyEmailComponent, ForgotPasswordComponent, ResetPasswordComponent],
    imports: [
        CommonModule,
        FormsModule,
        WidgetsModule,
        AppRoutingModule
    ]
})
export class AuthModule {
}
