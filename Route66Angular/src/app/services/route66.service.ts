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

    if(this.terminalFeed == null) {

      const apiUrl = this.fetchApiUrl()
      if(apiUrl) {

        const pollUrl = `${ apiUrl }/poll`
        let force = true;

        this.terminalFeed = interval(5000).pipe(
          startWith(0),
          switchMap(() => this.httpClient.get(pollUrl, { withCredentials: true })),
          filter(result => {
            const fetch = force || result as boolean
            force = false
            return fetch
          }),
          switchMap(() => this.httpClient.get(apiUrl, { withCredentials: true })),
          map(response => response as FieldData[][])
        )
      }
    }

    this.terminalFeed?.subscribe(observer)
  }

  sendKey(aid : Aid) {

    const apiUrl = this.fetchApiUrl()
    if(!apiUrl) {
      return Promise.reject("null apiUrl")
    }

    const fieldSubmission : Partial<FieldSubmission> = {
      aid: aid
    }

    return firstValueFrom(this.httpClient.post(apiUrl!, fieldSubmission, { withCredentials: true }).pipe(
      delay(2000),
      switchMap(() => this.httpClient.get(apiUrl!, { withCredentials: true })),
      map(response => response as FieldData[][]),
    ))
  }

  sendFields(aid : Aid, cursorRow : number, cursorCol : number, fieldData : FieldData[]) {

    const apiUrl = this.fetchApiUrl()
    if(!apiUrl) {
      return Promise.reject("null apiUrl")
    }

    const fieldSubmission : FieldSubmission = {
      aid: aid,
      cursorRow : cursorRow,
      cursorCol : cursorCol,
      fieldData : fieldData
    }

    return firstValueFrom(this.httpClient.post(apiUrl!, fieldSubmission, { withCredentials: true }).pipe(
      delay(2000),
      switchMap(() => this.httpClient.get(apiUrl!, { withCredentials: true })),
      map(response => response as FieldData[][]),
    ))
  }

  private fetchApiUrl() : string | null {
    const apiDataStr = sessionStorage.getItem(this.KEY_API_ADDRESS)
    if(apiDataStr == null) { return null; }
    const apiData = JSON.parse(apiDataStr) as ConnectionData
    const apiUrl = `https://${ apiData.address }:${ apiData.port }`

    return apiUrl
  }
}


