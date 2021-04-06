import {Component, OnInit} from '@angular/core';
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {Cuisine} from "../../../models/cuisine";
import {EditableCuisine} from "../../../models/editable-cuisine";
import {HttpClient} from "@angular/common/http";

@Component({
    selector: 'app-cuisines-list',
    templateUrl: './cuisines-list.component.html',
    styleUrls: ['./cuisines-list.component.scss']
})
export class CuisinesListComponent implements OnInit {
    items: EditableCuisine[] = [];
    showLoading = true;
    pageNumber: number = 1;
    totalItems!: number;
    newCuisine: string = "";

    constructor(private msiHttp: MsiHttpService, private http: HttpClient, private toastService: ToastService) {
    }

    ngOnInit(): void {
        this.loadItems();
    }

    loadItems() {
        this.showLoading = true;
        this.msiHttp.getPage<Cuisine>("/api/admin/cuisines/", this.pageNumber).subscribe(res => {
            this.showLoading = false;
            this.items = res.items.map(c => this.toEditableCuisine(c));
            this.pageNumber = res.page;
            this.totalItems = res.itemCount;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

    startEditing(cuisine: EditableCuisine) {
        cuisine.isEditing = true;
        cuisine.newName = cuisine.name;
    }

    save(cuisine: EditableCuisine) {
        this.showLoading = true;
        this.http.put<Cuisine>(`/api/admin/cuisines/${cuisine.cuisineId}/`, {cuisineId: cuisine.cuisineId, name: cuisine.newName}).subscribe(res => {
            cuisine.isEditing = false;
            cuisine.name = res.name;
            this.showLoading = false;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

    cancelEditing(cuisine: EditableCuisine) {
        cuisine.isEditing = false;
    }

    delete(cuisine: EditableCuisine) {
        this.showLoading = true;
        this.http.delete(`/api/admin/cuisines/${cuisine.cuisineId}`).subscribe(
            () => {
                this.items = this.items.filter(c => c.cuisineId != cuisine.cuisineId);
                this.showLoading = false;
            }, error => {
                this.showLoading = false;
                this.toastService.handleHttpError(error);
            }
        )
    }

    toEditableCuisine(c: Cuisine): EditableCuisine {
        return {...c, isEditing: false, newName: c.name};
    }

    createNew() {
        this.showLoading = true;
        this.http.post<Cuisine>("/api/admin/cuisines", {name: this.newCuisine}).subscribe(res => {
            this.items.push(this.toEditableCuisine(res));
            this.showLoading = false;
            this.newCuisine = "";
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        })
    }

    goToPage(pageNumber: number) {
        this.pageNumber = pageNumber;
        this.loadItems();
    }
}
