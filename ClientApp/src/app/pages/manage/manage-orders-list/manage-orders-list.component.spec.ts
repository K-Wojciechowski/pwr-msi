import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageOrdersListComponent } from './manage-orders-list.component';

describe('ManageOrdersListComponent', () => {
  let component: ManageOrdersListComponent;
  let fixture: ComponentFixture<ManageOrdersListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageOrdersListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageOrdersListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
