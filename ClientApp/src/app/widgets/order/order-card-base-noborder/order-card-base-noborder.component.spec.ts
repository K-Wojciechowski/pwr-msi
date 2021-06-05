import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardBaseNoborderComponent } from './order-card-base-noborder.component';

describe('OrderCardBaseNoborderComponent', () => {
  let component: OrderCardBaseNoborderComponent;
  let fixture: ComponentFixture<OrderCardBaseNoborderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardBaseNoborderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardBaseNoborderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
