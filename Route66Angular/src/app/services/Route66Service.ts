import {HttpClient} from "@angular/common/http";
import {ConnectionData} from "../models/connection-data";
import {Injectable} from "@angular/core";
import { lastValueFrom } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class Route66Service {

  constructor(private httpClient : HttpClient) {}

  connect(apiData : ConnectionData, tn3270Data: ConnectionData) {
    const apiUrl = `https://${apiData.address}:${apiData.port}/connection`
    return lastValueFrom(this.httpClient.post(
      apiUrl,
      tn3270Data,
      {withCredentials: true}));
  }
}
