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
import {AuthType} from "./models/enum/auth-type";
import {OrdersOverviewComponent} from "./pages/orders/orders-overview/orders-overview.component";
import {OrdersDetailsComponent} from "./pages/orders/orders-details/orders-details.component";
import {PaymentsOverviewComponent} from "./pages/orders/payments-overview/payments-overview.component";
import {PaymentsInfoComponent} from "./pages/orders/payments-info/payments-info.component";
import {PaymentsMakeComponent} from "./pages/orders/payments-make/payments-make.component";
import {PaymentsCheckComponent} from "./pages/orders/payments-check/payments-check.component";
import {AuthGuardService} from "./services/auth-guard.service";
import {ManagePickContextComponent} from "./pages/manage/manage-pick-context/manage-pick-context.component";
import {ManageIndexComponent} from "./pages/manage/manage-index/manage-index.component";
import {ManageMenuCategoriesComponent} from "./pages/manage/manage-menu-categories/manage-menu-categories.component";
import {ManageMenuItemsComponent} from "./pages/manage/manage-menu-items/manage-menu-items.component";
import {RestaurantMenuComponent} from "./pages/browse/restaurant-menu/restaurant-menu.component";
import {OrdersMakePaymentComponent} from "./pages/orders/orders-make-payment/orders-make-payment.component";
import {OrdersListComponent} from "./pages/delivery/orders-list/orders-list.component";
import {OrderDetailsComponent} from "./pages/delivery/order-details/order-details.component";
import {RestaurantsOrderListComponent} from "./pages/delivery/restaurants-order-list/restaurants-order-list.component";
import {ManageOrdersListComponent} from "./pages/manage/manage-orders-list/manage-orders-list.component";
import {ManageOrdersDetailsComponent} from "./pages/manage/manage-orders-details/manage-orders-details.component";
import {ProfileEditComponent} from "./pages/account/profile-edit/profile-edit.component";
import {AddressListComponent} from "./pages/account/address-list/address-list.component";
import {AddressEditComponent} from "./pages/account/address-edit/address-edit.component";
import {AddressAddComponent} from "./pages/account/address-add/address-add.component";

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
    {path: "browse/:id", component: RestaurantMenuComponent, data: {hideSidebar: true, sidebar: null, auth: AuthType.USER}},
    {path: "orders", component: OrdersOverviewComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "orders/:id", component: OrdersDetailsComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "orders/:id/pay", component: OrdersMakePaymentComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "payments", component: PaymentsOverviewComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "payments/repay", component: PaymentsMakeComponent, data: {sidebar: "orders", isBalanceRepayment: true, auth: AuthType.USER}},
    {path: "payments/:id", component: PaymentsInfoComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "payments/:id/make", component: PaymentsMakeComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "payments/:id/check", component: PaymentsCheckComponent, data: {sidebar: "orders", auth: AuthType.USER}},
    {path: "manage", component: ManagePickContextComponent, data: {sidebar: null}},
    {path: "manage/:restaurantId/start", component: ManageIndexComponent, data: {sidebar: "manage", auth: AuthType.ACCEPT_OR_MANAGE}},
    {path: "manage/:restaurantId/orders", component: ManageOrdersListComponent, data: {sidebar: "manage", auth: AuthType.ACCEPT}},
    {path: "manage/:restaurantId/orders/:id", component: ManageOrdersDetailsComponent, data: {sidebar: "manage", auth: AuthType.ACCEPT}},
    {path: "manage/:restaurantId/menucategories", component: ManageMenuCategoriesComponent, data: {sidebar: "manage", auth: AuthType.MANAGE}},
    {path: "manage/:restaurantId/menu", component: ManageMenuItemsComponent, data: {sidebar: "manage", auth: AuthType.MANAGE}},
    {path: "deliver/orders/:restaurantId", component: OrdersListComponent, data: {sidebar: "delivery", auth: AuthType.USER}},
    {path: "deliver/restaurants", component: RestaurantsOrderListComponent, data: {sidebar: "delivery", auth: AuthType.USER}},
    {path: "deliver/order/:id", component: OrderDetailsComponent, data: {sidebar: "delivery", auth: AuthType.USER}},
    {path: "account", redirectTo: "/account/profile", pathMatch: "full"},
    {path: "account/profile", component: ProfileEditComponent, data: {sidebar: "account", auth: AuthType.USER}},
    {path: "account/address", component: AddressListComponent, data: {sidebar: "account", auth: AuthType.USER}},
    {path: "account/address/add", component: AddressAddComponent, data: {sidebar: "account", auth: AuthType.USER}},
    {path: "account/address/:id", component: AddressEditComponent, data: {sidebar: "account", auth: AuthType.USER}},
];

const routesWithActivators = routes.map(route => {
    return (route.redirectTo !== undefined) ? route : {
    ...route,
    canActivate: [AuthGuardService]
}});

@NgModule({
  imports: [RouterModule.forRoot(routesWithActivators)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
