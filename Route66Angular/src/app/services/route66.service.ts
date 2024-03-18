import { HttpClient } from "@angular/common/http";
import { ConnectionData } from "../models/connection-data";
import { Injectable } from "@angular/core";
import {
  delay,
  filter,
  firstValueFrom,
  interval,
  lastValueFrom,
  map,
  Observable,
  Observer,
  startWith,
  switchMap
} from "rxjs";
import { FieldData } from "../models/field-data";
import { Aid } from "../models/aid";
import { FieldSubmission } from "../models/field-submission";

@Injectable({
  providedIn: 'root'
})
export class Route66Service {

  private readonly KEY_API_ADDRESS = "apiAddress";
  terminalFeed : Observable<FieldData[][]> | null = null;

  constructor(private httpClient : HttpClient) {
  }

  connect(apiData : ConnectionData, tn3270Data : ConnectionData) {

    sessionStorage.setItem(this.KEY_API_ADDRESS, JSON.stringify(apiData));
    const apiUrl = `https://${ apiData.address }:${ apiData.port }/connection`
    return lastValueFrom(this.httpClient.post(
      apiUrl,
      tn3270Data,
      { withCredentials: true }));
  }

  startTerminalPolling(observer : Observer<FieldData[][]>) : void {

    const apiDataStr = sessionStorage.getItem(this.KEY_API_ADDRESS);
    if(apiDataStr == null) { return; }
    const apiData = JSON.parse(apiDataStr) as ConnectionData;

    if(this.terminalFeed == null) {

      const pollUrl = `https://${ apiData.address }:${ apiData.port }/poll`
      const terminalUrl = `https://${ apiData.address }:${ apiData.port }/`
      let force = true;

      this.terminalFeed = interval(30000).pipe(
        startWith(0),
        switchMap(() => this.httpClient.get(pollUrl, { withCredentials: true })),
        filter(result => {
          const fetch = force || result as boolean
          force = false
          return fetch
        }),
        switchMap(() => this.httpClient.get(terminalUrl, { withCredentials: true })),
        map(response => response as FieldData[][])
      )
    }

    this.terminalFeed.subscribe(observer)
  }

  sendKey(aid : Aid) {

    const apiDataStr = sessionStorage.getItem(this.KEY_API_ADDRESS)
    if(apiDataStr == null) { return; }
    const apiData = JSON.parse(apiDataStr) as ConnectionData
    const apiUrl = `https://${ apiData.address }:${ apiData.port }/`

    const fieldSubmission : Partial<FieldSubmission> = {
      aid: aid
    }

    return firstValueFrom(this.httpClient.post(apiUrl, fieldSubmission, { withCredentials: true }).pipe(
      delay(2000),
      switchMap(() => this.httpClient.get(apiUrl, { withCredentials: true })),
      map(response => response as FieldData[][]),
    ))
  }
}


