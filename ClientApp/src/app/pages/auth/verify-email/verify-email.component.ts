import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";

@Component({
    selector: 'app-verify-email',
    templateUrl: './verify-email.component.html',
    styleUrls: ['./verify-email.component.scss']
})
export class VerifyEmailComponent implements OnInit {
    showLoading: boolean = true;
    showError: boolean = false;

    constructor(private http: HttpClient, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.verifyEmailFromRoute();
    }


    verifyEmailFromRoute() {
        const token = this.route.snapshot.paramMap.get("token");
        if (token === null) {
            this.showLoading = false;
            this.showError = true;
        } else {
            this.verifyEmail(token);
        }
    }

    verifyEmail(token: string) {
        this.showLoading = true;
        this.http.post(`/api/auth/verify/${token}`, null).subscribe(() => {
            this.showError = false;
            this.showLoading = false;
        }, (error) => {
            console.log(error);
            this.showError = true;
            this.showLoading = false;
        })
    }

}
