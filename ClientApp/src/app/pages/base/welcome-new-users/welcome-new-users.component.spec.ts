import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WelcomeNewUsersComponent } from './welcome-new-users.component';

describe('WelcomeNewUsersComponent', () => {
  let component: WelcomeNewUsersComponent;
  let fixture: ComponentFixture<WelcomeNewUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WelcomeNewUsersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WelcomeNewUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
