import {Injectable} from '@angular/core';
import {Toast, ToastType} from "./toast.types";

@Injectable({
    providedIn: 'root'
})
export class ToastService {
    toasts: Toast[] = [];
    constructor() {
    }

    show(toast: Toast) {
        this.toasts.push(toast);
    }

    showSuccess(message: string) {
        this.show({title: "", message, type: ToastType.SUCCESS});
    }

    showError(message: string) {
        this.show({title: "Error", message, type: ToastType.DANGER});
    }

    getClass(toastType: ToastType | undefined): string {
        switch (toastType) {
            case ToastType.SUCCESS:
                return 'bg-success text-light';
            case ToastType.DANGER:
                return 'bg-danger text-light';
            default:
                return '';
        }
    }

    remove(toast: Toast) {
        this.toasts = this.toasts.filter(t => t != toast);
    }

    handleHttpError(error: any) {
        this.showError("Operation failed! Error: " + error.message);
    }
}
