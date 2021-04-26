import {ComponentFixture, TestBed} from '@angular/core/testing';

import {HeadingButtonsComponent} from './heading-buttons.component';

describe('HeadingButtonsComponent', () => {
  let component: HeadingButtonsComponent;
  let fixture: ComponentFixture<HeadingButtonsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HeadingButtonsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HeadingButtonsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
