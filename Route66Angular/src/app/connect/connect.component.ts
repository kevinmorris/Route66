import { Component } from '@angular/core';
import {FormBuilder, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ConnectionData} from "../models/connection-data";
import {Route66Service} from "../services/Route66Service";
import {NgIf} from "@angular/common";

@Component({
  selector: 'app-connect',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgIf
  ],
  templateUrl: './connect.component.html',
  styleUrl: './connect.component.css'
})
export class ConnectComponent {

  apiConnectionForm = this.formBuilder.group<ConnectionData>({
    address: '',
    port: 0
  });

  tn3270ConnectionForm = this.formBuilder.group<ConnectionData>({
    address: '',
    port: 0
  });

  connectSuccess = false;

  constructor(
    private route66Service: Route66Service,
    private formBuilder: FormBuilder) {}

  async submit() {
    const result = await this.route66Service.connect(
      this.apiConnectionForm.getRawValue(),
      this.tn3270ConnectionForm.getRawValue())
    this.connectSuccess = true;
  }
}
