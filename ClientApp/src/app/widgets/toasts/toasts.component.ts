import { Component, OnInit } from '@angular/core';
import {ToastService} from "../../services/toast.service";
import {Toast, ToastType} from "../../services/toast.types";

@Component({
  selector: 'app-toasts',
  templateUrl: './toasts.component.html',
  styleUrls: ['./toasts.component.scss']
})
export class ToastsComponent implements OnInit {

  constructor(private toastService: ToastService) { }

  ngOnInit(): void {
  }

  get toasts(): Toast[] {
      return this.toastService.toasts;
  }

  getClass(toastType: ToastType | undefined): string {
      return this.toastService.getClass(toastType);
  }

    removeToast(toast: Toast) {
        this.toastService.remove(toast);
    }
}
