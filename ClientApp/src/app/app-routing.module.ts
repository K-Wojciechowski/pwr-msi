import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {IndexComponent} from "./pages/base/index/index.component";
import {LogInComponent} from "./pages/auth/log-in/log-in.component";
import {VerifyEmailComponent} from "./pages/auth/verify-email/verify-email.component";
import {RegisterComponent} from "./pages/auth/register/register.component";
import {ResetPasswordComponent} from "./pages/auth/reset-password/reset-password.component";
import {ForgotPasswordComponent} from "./pages/auth/forgot-password/forgot-password.component";
import {UsersListComponent} from "./pages/admin/users-list/users-list.component";
import {UsersAddComponent} from "./pages/admin/users-add/users-add.component";
import {UsersEditComponent} from "./pages/admin/users-edit/users-edit.component";
import {RestaurantsListComponent} from "./pages/admin/restaurants-list/restaurants-list.component";
import {RestaurantsAddComponent} from "./pages/admin/restaurants-add/restaurants-add.component";
import {RestaurantsEditComponent} from "./pages/admin/restaurants-edit/restaurants-edit.component";
import {CuisinesListComponent} from "./pages/admin/cuisines-list/cuisines-list.component";
import {AuthType} from "./models/auth-type";

const routes: Routes = [
    {path: "", component: IndexComponent, pathMatch: "full", data: {sidebar: null}},
    {path: "auth/login", component: LogInComponent, data: {sidebar: null}},
    {path: "auth/register", component: RegisterComponent, data: {sidebar: null}},
    {path: "auth/verify/:token", component: VerifyEmailComponent, data: {hideNavbar: true, sidebar: null}},
    {path: "auth/forgot", component: ForgotPasswordComponent, data: {hideNavbar: true, sidebar: null}},
    {path: "auth/reset/:token", component: ResetPasswordComponent, data: {hideNavbar: true, sidebar: null}},
    {path: "admin", redirectTo: "/admin/users", pathMatch: "full"},
    {path: "admin/users", component: UsersListComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
    {path: "admin/users/add", component: UsersAddComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
    {path: "admin/users/:id", component: UsersEditComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
    {path: "admin/restaurants", component: RestaurantsListComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
    {path: "admin/restaurants/add", component: RestaurantsAddComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
    {path: "admin/restaurants/:id", component: RestaurantsEditComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
    {path: "admin/cuisines", component: CuisinesListComponent, data: {sidebar: "admin", auth: AuthType.ADMIN}},
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: true})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
