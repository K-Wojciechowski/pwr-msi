export interface Toast {
    title: string;
    message: string;
    delay?: number;
    type?: ToastType;
}

export enum ToastType {
    NORMAL, SUCCESS, DANGER
}
