import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UsersRestaurantsEditorComponent} from './users-restaurants-editor.component';

describe('UsersRestaurantsEditorComponent', () => {
    let component: UsersRestaurantsEditorComponent;
    let fixture: ComponentFixture<UsersRestaurantsEditorComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [UsersRestaurantsEditorComponent]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(UsersRestaurantsEditorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
