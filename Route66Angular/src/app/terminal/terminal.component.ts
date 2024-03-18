import { Component } from '@angular/core';
import { Route66Service } from "../services/route66.service";
import { FieldData } from "../models/field-data";
import { RowComponent } from "../row/row.component";
import { Observer } from "rxjs";
import { NgForOf } from "@angular/common";
import { Aid } from "../models/aid";

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
      this.terminalFeedObserver(fd => {
        this.fieldData = fd
      })
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

  reset() {

  }

  functionKey(aid : Aid) {
    this.route66Service.sendKey(aid)?.then(fd => {
      if (fd) {
        this.fieldData = fd;
      }
    })
  }

  protected readonly Aid = Aid;
}

type FieldDataSetter = (fd: FieldData[][]) => void



