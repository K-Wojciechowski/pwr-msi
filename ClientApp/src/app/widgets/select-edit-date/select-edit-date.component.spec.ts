import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectEditDateComponent } from './select-edit-date.component';

describe('SelectEditDateComponent', () => {
  let component: SelectEditDateComponent;
  let fixture: ComponentFixture<SelectEditDateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectEditDateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectEditDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
