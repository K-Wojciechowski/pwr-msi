import {ComponentFixture, TestBed} from '@angular/core/testing';

import {DashboardAppBoxComponent} from './dashboard-app-box.component';

describe('DashboardAppBoxComponent', () => {
    let component: DashboardAppBoxComponent;
    let fixture: ComponentFixture<DashboardAppBoxComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [DashboardAppBoxComponent]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(DashboardAppBoxComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
