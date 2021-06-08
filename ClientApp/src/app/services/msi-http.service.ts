import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {Page} from "../models/page";

interface HttpOptions {
    body?: any;
    headers?: HttpHeaders | {
        [header: string]: string | string[];
    };
    observe?: 'body';
    params?: HttpParams;
    reportProgress?: boolean;
    responseType?: 'json';
    withCredentials?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MsiHttpService {

  constructor(private http: HttpClient) { }

  getPage<T>(endpoint: string, page: number, options?: HttpOptions): Observable<Page<T>> {
      const updatedOptions = Object.assign({}, options);
      if (updatedOptions.params === undefined || updatedOptions.params === null) {
          updatedOptions.params = new HttpParams({fromObject: {"page": page.toString()}});
      } else {
          updatedOptions.params = updatedOptions.params.set("page", page.toString());
      }
      return this.http.get<Page<T>>(endpoint, updatedOptions);
  }
}
