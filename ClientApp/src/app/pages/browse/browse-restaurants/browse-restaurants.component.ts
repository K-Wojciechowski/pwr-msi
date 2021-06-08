import {Component, OnInit, TemplateRef} from '@angular/core';
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Address} from "../../../models/address";
import {RestaurantFull} from "../../../models/restaurant-full";
import {NgbModal, NgbModalRef} from "@ng-bootstrap/ng-bootstrap";
import {CuisineWrapper} from "../../../models/cuisine-wrapper";
import {Cuisine} from "../../../models/cuisine";

@Component({
    selector: 'app-browse-restaurants',
    templateUrl: './browse-restaurants.component.html',
    styleUrls: ['./browse-restaurants.component.scss']
})
export class BrowseRestaurantsComponent implements OnInit {
    items: RestaurantFull[] = [];
    showLoading: number = 0;
    pageNumber: number = 1;
    totalItems!: number;
    query: string = "";
    useAddress = true;
    addresses: Address[] = [];
    address: Address | null = null;
    latitude: number | null = null;
    longitude: number | null = null;
    addressModal: NgbModalRef | null = null;
    cuisines: CuisineWrapper[] = [];

    constructor(private http: HttpClient, private msiHttp: MsiHttpService, private toastService: ToastService, private modalService: NgbModal) {
    }

    ngOnInit(): void {
        this.loadItems();
        this.loadCuisines();
        this.loadAddresses();
    }

    setLatLon(params: HttpParams, source: {latitude: number | undefined | null, longitude: number | undefined | null}): HttpParams {
        return params.set("latitude", source.latitude?.toString() ?? "0").set("longitude", source.longitude?.toString() ?? "0");
    }

    loadItems() {
        this.showLoading++;
        const cuisineSearch = this.cuisines.filter(c => c.isChecked).map(c => c.v.cuisineId).join(",");
        let params = new HttpParams().set("search", this.query).set("cuisines", cuisineSearch);
        params = this.setLatLon(params, this.useAddress ? (this.address ?? this) : this);
        this.msiHttp.getPage<RestaurantFull>("/api/offer/restaurants/", this.pageNumber, {params: params})
            .subscribe(res => {
                this.showLoading--;
                this.items = res.items;
                this.pageNumber = res.page;
                this.totalItems = res.itemCount;
            }, error => {
                this.showLoading--;
                this.toastService.handleHttpError(error);
            });
    }

    loadCuisines() {
        this.http.get<Cuisine[]>("/api/offer/restaurants/cuisines/")
            .subscribe(res => {
                this.showLoading--;
                this.cuisines = res.map(c => new CuisineWrapper(c));
            }, error => {
                this.showLoading--;
                this.toastService.handleHttpError(error);
            });
    }

    updateQuery(query: string) {
        this.query = query;
        this.loadItems();
    }

    loadAddresses() {
        this.http.get<Address[]>(`/api/addresses/all/`).subscribe(a => {
            this.addresses = a;
            --this.showLoading;
        }, err => {
            this.toastService.handleHttpError(err);
            --this.showLoading;
        });
        this.http.get<Address | null>(`/api/addresses/default/`).subscribe(a => {
            if (a !== null) {
                this.address = a;
            }
            --this.showLoading;
        }, err => {
            this.toastService.handleHttpError(err);
            --this.showLoading;
        });
    }

    switchToAddress() {
        this.useAddress = true;
        this.loadItems();
    }

    switchToLocation() {
        navigator.geolocation.getCurrentPosition((pos) => {
            this.useAddress = false;
            this.latitude = pos.coords.latitude;
            this.longitude = pos.coords.longitude;
            this.loadItems();
        });
    }

    getCuisines(restaurant: RestaurantFull) {
        return restaurant.cuisines.map(c => c.name).join(", ");
    }

    getAddress(restaurant: RestaurantFull) {
        return [restaurant.address.firstLine, restaurant.address.secondLine].map(l => l.trim()).filter(l => l.length > 0).join(", ");
    }

    goToPage(pageNumber: number) {
        this.pageNumber = pageNumber;
        this.loadItems();
    }

    openAddressPicker(addressModal: TemplateRef<any>) {
        this.reloadAddressesSilently();
        this.addressModal = this.modalService.open(addressModal, {size: 'lg'});
    }

    selectAddress(address: Address) {
        this.address = address;
        this.addressModal?.close("Selected");
        this.loadItems();
    }

    setCuisine(cuisine: CuisineWrapper, value: boolean) {
        console.log(cuisine, value);
        cuisine.isChecked = value;
        this.loadItems();
    }

    reloadAddressesSilently() {
        this.http.get<Address[]>(`/api/addresses/all/`).subscribe(a => {
            this.addresses = a;
        }, err => {
            this.toastService.handleHttpError(err);
        });
    }
}
