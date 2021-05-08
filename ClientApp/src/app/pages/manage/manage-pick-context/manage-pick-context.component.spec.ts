import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagePickContextComponent } from './manage-pick-context.component';

describe('ManagePickContextComponent', () => {
  let component: ManagePickContextComponent;
  let fixture: ComponentFixture<ManagePickContextComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManagePickContextComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManagePickContextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
