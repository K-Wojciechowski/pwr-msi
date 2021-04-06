import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IndexComponent } from './index.component';
import {WelcomeNewUsersComponent} from "../welcome-new-users/welcome-new-users.component";

describe('IndexComponent', () => {
  let component: IndexComponent;
  let fixture: ComponentFixture<IndexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IndexComponent, WelcomeNewUsersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
