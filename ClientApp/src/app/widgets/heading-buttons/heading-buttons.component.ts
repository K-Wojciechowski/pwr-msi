import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-heading-buttons',
  templateUrl: './heading-buttons.component.html',
  styleUrls: ['./heading-buttons.component.scss']
})
export class HeadingButtonsComponent {
    @Input("name") name!: string;

  constructor() { }
}
