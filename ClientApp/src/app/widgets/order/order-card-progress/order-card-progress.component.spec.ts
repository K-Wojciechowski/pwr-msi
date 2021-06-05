import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardProgressComponent } from './order-card-progress.component';

describe('OrderCardProgressComponent', () => {
  let component: OrderCardProgressComponent;
  let fixture: ComponentFixture<OrderCardProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardProgressComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
