import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageMenuItemsComponent } from './manage-menu-items.component';

describe('ManageMenuItemsComponent', () => {
  let component: ManageMenuItemsComponent;
  let fixture: ComponentFixture<ManageMenuItemsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageMenuItemsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageMenuItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
