import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../services/toast.service";
import {ResultDto} from "../../models/result-dto";

@Component({
    selector: 'app-image-upload',
    templateUrl: './image-upload.component.html',
    styleUrls: ['./image-upload.component.scss']
})
export class ImageUploadComponent implements OnInit {
    @ViewChild("fileInput", {static: true}) fileInput!: ElementRef<HTMLInputElement>;
    @Input("imageUrl") imageUrl!: string | null;
    @Input("imageAlt") imageAlt!: string;
    @Input("endpoint") endpoint!: string;
    @Output("urlChanged") urlChanged: EventEmitter<string | null> = new EventEmitter();
    showLoading: boolean = false;

    constructor(private http: HttpClient, private toastService: ToastService) {
    }

    ngOnInit(): void {
    }

    upload() {
        this.fileInput.nativeElement.click();
    }

    delete() {
        this.showLoading = true;
        this.http.delete(this.endpoint).subscribe(() => {
            this.urlChanged.emit(null);
            this.showLoading = false;
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }

    handleFile(event: Event) {
        const fileInput = event.target as HTMLInputElement;
        const files = fileInput.files;
        if (files == null || files.length !== 1) return;
        const file = files[0];
        const formData = new FormData();
        formData.append("file", file);

        this.showLoading = true;
        this.http.post<ResultDto<string>>(this.endpoint, formData).subscribe(res => {
            this.urlChanged.emit(res.result);
            this.showLoading = false;
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }
}
