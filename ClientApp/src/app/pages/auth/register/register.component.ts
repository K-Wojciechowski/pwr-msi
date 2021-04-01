import {Component, OnInit} from '@angular/core';
import {NgForm} from "@angular/forms";
import {every, values} from "lodash";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
    showLoading = false;
    showSuccess = false;

    constructor(private http: HttpClient, private toastService: ToastService) {
    }

    ngOnInit(): void {
    }

    canSubmit(f: NgForm) {
        return every(values(f.controls), c => c.valid);
    }

    submit(f: NgForm) {
        const body = f.value;
        this.showLoading = true;
        this.http.post("/api/auth/register/", body).subscribe(
            () => {
                this.showSuccess = true;
                this.showLoading = false;
            }, (err) => {
                console.log(err);
                this.showLoading = false;
                this.toastService.showError("Registration failed, please make sure your data is correct.");
            }
        )
        return;
    }
}
