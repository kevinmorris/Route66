import { HttpClient } from "@angular/common/http";
import { ConnectionData } from "../models/connection-data";
import { Injectable } from "@angular/core";
import { filter, interval, lastValueFrom, map, Observable, Observer, of, Subscription, switchMap } from "rxjs";
import { FieldData } from "../models/field-data";

@Injectable({
  providedIn: 'root'
})
export class Route66Service {

  apiData : ConnectionData | null = null;
  terminalFeed : Observable<FieldData[][]> | null = null;

  constructor(private httpClient : HttpClient) {
  }

  connect(apiData : ConnectionData, tn3270Data : ConnectionData) {

    this.apiData = apiData;
    const apiUrl = `https://${ apiData.address }:${ apiData.port }/connection`
    return lastValueFrom(this.httpClient.post(
      apiUrl,
      tn3270Data,
      { withCredentials: true }));
  }

  startTerminalPolling(observer : Observer<FieldData[][]>) : void {

    if(this.apiData == null) { return; }
    if(this.terminalFeed == null) {

      const pollUrl = `https://${ this.apiData.address }:${ this.apiData.port }/poll`
      const terminalUrl = `https://${ this.apiData.address }:${ this.apiData.port }/`

      this.terminalFeed = interval(5000).pipe(
        switchMap(() => this.httpClient.get(pollUrl, { withCredentials: true })),
        filter(result => result as boolean),
        switchMap(() => this.httpClient.get(terminalUrl, { withCredentials: true })),
        map(response => response as FieldData[][])
      )
    }

    this.terminalFeed.subscribe(observer)
  }
}
