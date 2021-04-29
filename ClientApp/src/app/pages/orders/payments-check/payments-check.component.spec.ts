import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentsCheckComponent } from './payments-check.component';

describe('PaymentsCheckComponent', () => {
  let component: PaymentsCheckComponent;
  let fixture: ComponentFixture<PaymentsCheckComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PaymentsCheckComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentsCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
