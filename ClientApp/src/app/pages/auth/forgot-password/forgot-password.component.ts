import {Component, OnInit} from '@angular/core';
import {NgForm} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";

@Component({
    selector: 'app-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
    showLoading: boolean = false;
    showSuccess: boolean = false;

    constructor(private http: HttpClient, private toastService: ToastService) {
    }

    ngOnInit(): void {
    }

    canSubmit(f: NgForm) {
        return f.value.email !== undefined && f.value.email.length > 0;
    }

    submit(f: NgForm) {
        this.showLoading = true;
        this.http.post("/api/auth/forgot", f.value).subscribe(
            () => {
                this.showLoading = false;
                this.showSuccess = true;
            }, error => {
                this.showLoading = false;
                console.log(error);
                this.toastService.showError("Failed to request reset.")
            }
        );
    }
}
