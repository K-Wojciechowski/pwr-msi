import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentsMakeComponent } from './payments-make.component';

describe('PaymentsMakeComponent', () => {
  let component: PaymentsMakeComponent;
  let fixture: ComponentFixture<PaymentsMakeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PaymentsMakeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentsMakeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
