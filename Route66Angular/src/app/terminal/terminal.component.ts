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
  cursor : [number, number] | undefined

  constructor(private route66Service: Route66Service) {
    this.fieldData = [];
  }

  onFocusChanged(cursor : [number, number]) {
    this.cursor = cursor
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

  enter() {
    this.sendUserData(Aid.ENTER)
  }

  functionKey(aid : Aid) {
    this.route66Service.sendKey(aid)?.then(fd => {
      this.fieldData = fd;
    })
  }
  sendUserData(aid : Aid) {

    let fields : FieldData[] = []
    fields = fields.concat(...this.fieldData)

    const inputFields = fields.filter(f => !f.isProtected)
    const cursorField = inputFields.find(f => {
      return this.cursor && this.cursor[0] == f.row && this.cursor[1] == f.col
    }) ?? inputFields.slice(-1)[0];

    (() => {
      if(this.cursor) {
        const cursorRow = this.cursor[0]
        const cursorCol = this.cursor[1] + cursorField.value.length

        return this.route66Service.sendFields(aid, cursorRow, cursorCol, inputFields)
      } else {
        return this.route66Service.sendKey(aid)
      }
    })().then((fd : FieldData[][]) => {
      this.fieldData = fd
    })
  }

  protected readonly Aid = Aid;
}

type FieldDataSetter = (fd: FieldData[][]) => void



