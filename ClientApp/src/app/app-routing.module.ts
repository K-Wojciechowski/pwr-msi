import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {IndexComponent} from "./pages/base/index/index.component";
import {LogInComponent} from "./pages/auth/log-in/log-in.component";

const routes: Routes = [
    {path: "", component: IndexComponent, pathMatch: "full", data: {hideSidebar: true}},
    {path: "auth/login", component: LogInComponent, data: {hideNavbar: true, hideSidebar: true}}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {enableTracing: true})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
