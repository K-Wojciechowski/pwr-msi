import { Component, OnInit } from '@angular/core';
import {AuthStoreService} from "../../../services/auth-store.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    public canDeliver: boolean = false;
    public canManage: boolean = false;
    public isAdmin: boolean = false;

  constructor(private authStore: AuthStoreService) { }

  ngOnInit(): void {
      this.authStore.access.subscribe(access => {
          this.canDeliver = access !== null && access !== undefined && access.deliver.length > 0;
          this.canManage = access !== null && access !== undefined && (access.manage.length > 0 || access.accept.length > 0);
          this.isAdmin = !!access?.admin;
      });
  }

}
