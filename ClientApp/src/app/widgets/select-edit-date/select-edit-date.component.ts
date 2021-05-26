import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {NgbDateStruct, NgbTimeStruct} from "@ng-bootstrap/ng-bootstrap";
import {DateTime, Duration} from "luxon";

@Component({
    selector: 'app-select-edit-date',
    templateUrl: './select-edit-date.component.html',
    styleUrls: ['./select-edit-date.component.scss']
})
export class SelectEditDateComponent implements OnInit, OnChanges {
    public date!: NgbDateStruct;
    public time!: NgbTimeStruct;
    public selectedDt: DateTime | null = null;
    @Output("selectedDate") selectedDate = new EventEmitter<DateTime | null>();
    @Input("latestDate") latestDate: DateTime | null = null;

    constructor() {
        this.updateDateTime();
    }

    ngOnInit(): void {
        this.updateDateTime();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.latestDate.previousValue !== changes.latestDate.currentValue) {
           this.updateDateTime();
        }
    }

    private updateDateTime() {
        if (this.latestDate !== null) {
            const dt = this.latestDate.toLocal();
            this.date = {day: dt.day, month: dt.month, year: dt.year};
            this.time = {hour: dt.hour, minute: dt.minute, second: dt.second};
        } else {
            const dt = DateTime.now().plus(Duration.fromObject({days: 1}));
            this.date = {day: dt.day, month: dt.month, year: dt.year};
            this.time = {hour: 4, minute: 0, second: 0};
        }
    }

    public submit() {
        this.selectedDt = DateTime.fromObject({...this.date, ...this.time});
        this.selectedDate.emit(this.selectedDt);
    }

    public unselect() {
        this.selectedDt = null;
        this.selectedDate.emit(this.selectedDt);
    }
}
