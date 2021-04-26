import {ComponentFixture, TestBed} from '@angular/core/testing';

import {RestaurantsAddComponent} from './restaurants-add.component';

describe('RestaurantsAddComponent', () => {
    let component: RestaurantsAddComponent;
    let fixture: ComponentFixture<RestaurantsAddComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [RestaurantsAddComponent]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(RestaurantsAddComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
