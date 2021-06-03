import { Component, OnInit } from '@angular/core';
import {RestaurantBasic} from "../../../models/restaurant-basic";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {Toast} from "../../../services/toast.types";

@Component({
  selector: 'app-browse-restaurants',
  templateUrl: './browse-restaurants.component.html',
  styleUrls: ['./browse-restaurants.component.scss']
})
export class BrowseRestaurantsComponent implements OnInit {
    items: RestaurantBasic[] = [];
    showLoading = true;
    pageNumber: number = 1;
    totalItems!: number;
    pos: any;

  constructor(private msiHttp: MsiHttpService, private toastService: ToastService) { }

  ngOnInit(): void {
      this.getPosition();
      this.loadItems();
  }
    loadItems() {
        this.showLoading = true;
        //this.msiHttp.getPage<RestaurantBasic>("/api/offer/restaurants/all?lng="+this.pos.coords.longitude+"&lng="+this.pos.coords.latitude, this.pageNumber)
        this.msiHttp.getPage<RestaurantBasic>("/api/offer/restaurants/all/", this.pageNumber)
            .subscribe(res => {
            this.showLoading = false;
            this.items = res.items;
            this.pageNumber = res.page;
            this.totalItems = res.itemCount;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
        
    }
    getPosition = () => {
        navigator.geolocation.getCurrentPosition((pos) => {
           this.pos = pos;
        });
    }
 
    
    goToPage(pageNumber: number) {
        this.pageNumber = pageNumber;
        this.loadItems();
    }
    
    searchFor(){
      let phrase = (<HTMLInputElement>document.getElementById("search-bar")).value;
      if(phrase==""){
          this.loadItems();
      }
      else{
          this.showLoading = true;
          this.msiHttp.getPage<RestaurantBasic>("/api/offer/restaurants/search?phrase="+phrase, this.pageNumber).subscribe(res => {
              this.showLoading = false;
              this.items = res.items;
              this.pageNumber = res.page;
              this.totalItems = res.itemCount;
          }, error => {
              this.showLoading = false;
              this.toastService.handleHttpError(error);
          });
      }
    }

}
