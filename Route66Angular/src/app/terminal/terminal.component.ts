import { Component } from '@angular/core';
import { Route66Service } from "../services/Route66Service";
import { FieldData } from "../models/field-data";
import { RowComponent } from "../row/row.component";
import { Observer } from "rxjs";
import { NgForOf } from "@angular/common";

@Component({
  selector: 'app-terminal',
  standalone: true,
  imports: [
    RowComponent,
    NgForOf
  ],
  templateUrl: './terminal.component.html',
  styleUrl: './terminal.component.css'
})
export class TerminalComponent {

  fieldData : FieldData[][];
  constructor(private route66Service: Route66Service) {
    this.fieldData = [];
  }

  ngOnInit(): void {
    this.route66Service.startTerminalPolling(
      this.terminalFeedObserver(fd =>
        this.fieldData = fd
      )
    )
  }

  terminalFeedObserver(fieldDataSetter: FieldDataSetter) : Observer<FieldData[][]> {
    return {
      next(fieldData : FieldData[][]) {
        fieldDataSetter(fieldData)
      },
      error(err: any) {
        console.error(err);
      },
      complete() {}
    };
  }
}

type FieldDataSetter = (fd: FieldData[][]) => void


