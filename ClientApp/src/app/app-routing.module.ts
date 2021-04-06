import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {IndexComponent} from "./pages/base/index/index.component";
import {LogInComponent} from "./pages/auth/log-in/log-in.component";
import {VerifyEmailComponent} from "./pages/auth/verify-email/verify-email.component";
import {RegisterComponent} from "./pages/auth/register/register.component";
import {ResetPasswordComponent} from "./pages/auth/reset-password/reset-password.component";
import {ForgotPasswordComponent} from "./pages/auth/forgot-password/forgot-password.component";

const routes: Routes = [
    {path: "", component: IndexComponent, pathMatch: "full", data: {hideSidebar: true}},
    {path: "auth/login", component: LogInComponent, data: {hideSidebar: true}},
    {path: "auth/register", component: RegisterComponent, data: {hideSidebar: true}},
    {path: "auth/verify/:token", component: VerifyEmailComponent, data: {hideNavbar: true, hideSidebar: true}},
    {path: "auth/forgot", component: ForgotPasswordComponent, data: {hideNavbar: true, hideSidebar: true}},
    {path: "auth/reset/:token", component: ResetPasswordComponent, data: {hideNavbar: true, hideSidebar: true}},
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: true})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
