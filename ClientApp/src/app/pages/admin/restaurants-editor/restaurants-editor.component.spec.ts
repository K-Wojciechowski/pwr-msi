import {ComponentFixture, TestBed} from '@angular/core/testing';

import {RestaurantsEditorComponent} from './restaurants-editor.component';

describe('RestaurantsEditorComponent', () => {
    let component: RestaurantsEditorComponent;
    let fixture: ComponentFixture<RestaurantsEditorComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [RestaurantsEditorComponent]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(RestaurantsEditorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
