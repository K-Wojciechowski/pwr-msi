import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {NgForm} from "@angular/forms";
import {ActivatedRoute} from "@angular/router";

@Component({
    selector: 'app-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

    showLoading: boolean = true;
    showSuccess: boolean = false;
    showInvalid: boolean = false;
    private token: string = "";

    get endpoint() {
        return "/api/auth/reset/" + this.token;
    }

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute) {
    }


    verifyRequest() {
        this.showLoading = true;
        this.http.get(this.endpoint).subscribe(
            () => {
                this.showLoading = false;
            },
            (err) => {
                console.log(err);
                this.showLoading = false;
                this.showInvalid = true;
            });
    }

    ngOnInit(): void {
        const token = this.route.snapshot.paramMap.get("token");

        if (token === null) {
            this.showLoading = false;
            this.showInvalid = true;
        } else {
            this.token = token;
            this.verifyRequest();
        }
    }

    canSubmit(f: NgForm) {
        return f.controls.password.valid && f.controls.repeatPassword.valid;
    }

    submit(f: NgForm) {
        this.showLoading = true;
        this.http.post(this.endpoint, f.value).subscribe(
            () => {
                this.showLoading = false;
                this.showSuccess = true;
            }, error => {
                this.showLoading = false;
                console.log(error);
                this.toastService.showError("Failed to reset password.")
            }
        );
    }
}
