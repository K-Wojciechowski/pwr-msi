import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';

@Component({
    selector: 'app-progress-spinner-content',
    templateUrl: './progress-spinner-content.component.html',
    styleUrls: ['./progress-spinner-content.component.scss']
})
export class ProgressSpinnerContentComponent implements OnInit, OnChanges {
    @Input("showLoading") showLoading!: boolean;

    showGone = false;
    showSpinner = true;
    private timeout: number | null = null;

    constructor() {
    }

    ngOnInit(): void {
        this.startAnimation();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.showLoading.previousValue != changes.showLoading.currentValue) {
            this.startAnimation();
        }
    }

    startAnimation() {
        if (this.timeout !== null) {
            window.clearTimeout(this.timeout);
            this.timeout = null;
        }
        this.showSpinner = true;
        this.showGone = true;
        const timeout = this.showLoading ? 10 : 260;
        window.setTimeout(() => this.updateHideSpinner(), timeout);
    }

    updateHideSpinner() {
        this.showGone = false;
        this.showSpinner = this.showLoading;
    }
}
