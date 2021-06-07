import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardBaseComponent } from './order-card-base.component';

describe('OrderCardBaseComponent', () => {
  let component: OrderCardBaseComponent;
  let fixture: ComponentFixture<OrderCardBaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardBaseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardBaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
