import { Component } from '@angular/core';
import { Route66Service } from "../services/Route66Service";
import { FieldData } from "../models/field-data";
import { Observer } from "rxjs";

@Component({
  selector: 'app-terminal',
  standalone: true,
  imports: [],
  templateUrl: './terminal.component.html',
  styleUrl: './terminal.component.css'
})
export class TerminalComponent {



  constructor(private route66Service: Route66Service) {}

  ngOnInit(): void {
    this.route66Service.startTerminalPolling({
      next(fieldData : FieldData[][]) {
        console.info(fieldData);
      },
      error(err: any) {
        console.error(err);
      },
      complete() {}
    });
  }
}


