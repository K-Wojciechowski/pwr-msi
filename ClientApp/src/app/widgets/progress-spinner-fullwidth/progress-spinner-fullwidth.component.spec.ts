import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ProgressSpinnerFullwidthComponent} from './progress-spinner-fullwidth.component';

describe('ProgressSpinnerFullwidthComponent', () => {
  let component: ProgressSpinnerFullwidthComponent;
  let fixture: ComponentFixture<ProgressSpinnerFullwidthComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProgressSpinnerFullwidthComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgressSpinnerFullwidthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
