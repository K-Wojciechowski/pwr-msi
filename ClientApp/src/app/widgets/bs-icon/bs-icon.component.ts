import {Component, Input} from '@angular/core';

@Component({
  selector: 'bs-icon',
  templateUrl: './bs-icon.component.html',
  styleUrls: ['./bs-icon.component.scss']
})
export class BsIconComponent {
    @Input("name") public name!: string;

  constructor() { }
}
