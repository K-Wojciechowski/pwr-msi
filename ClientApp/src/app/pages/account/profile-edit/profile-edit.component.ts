import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute, Router} from "@angular/router";
import {UserProfile} from "../../../models/user-profile";

@Component({
    selector: 'app-profile-edit',
    templateUrl: './profile-edit.component.html',
    styleUrls: ['./profile-edit.component.scss']
})
export class ProfileEditComponent implements OnInit {
    showLoading: boolean = false;
    user: UserProfile | undefined;
    userId: number | undefined;
    billingAddressDefined: boolean = false;


    constructor(private http: HttpClient, private toastService: ToastService, private router: Router, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.loadUser()
    }

    loadUser() {
        this.showLoading = true;
        this.http.get<UserProfile>("/api/auth/profile/").subscribe(user => {
            this.user = user;
            this.showLoading = false;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
        this.billingAddressDefined = (this.user?.billingAddress != undefined)
    }

    editUser(user: UserProfile) {
        this.showLoading = true;
        this.http.put<UserProfile>("/api/auth/profile/edit/", user).subscribe(newUser => {
            this.showLoading = false;
            this.toastService.showSuccess("Your profile has been updated.");
            this.user = newUser;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

}
