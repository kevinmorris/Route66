import { Component } from '@angular/core';
import { Route66Service } from "../services/route66.service";
import { FieldData } from "../models/field-data";
import { RowComponent } from "../row/row.component";
import { Observer } from "rxjs";
import { NgForOf, NgIf } from "@angular/common";
import { Aid } from "../models/aid";

@Component({
  selector: 'app-terminal',
  standalone: true,
  imports: [
    RowComponent,
    NgForOf,
    NgIf
  ],
  templateUrl: './terminal.component.html',
  styleUrl: './terminal.component.css'
})
export class TerminalComponent {

  fieldData : FieldData[][];
  cursor : [number, number] | undefined
  wait : boolean
  errorMessage : string | undefined

  constructor(private route66Service: Route66Service) {
    this.fieldData = [];
    this.wait = false
  }

  onFocusChanged(cursor : [number, number]) {
    this.cursor = cursor
  }

  ngOnInit(): void {
    this.route66Service.startTerminalPolling(
      this.terminalFeedObserver((fd, errorMessage) => {
        if(fd != null) {
          this.fieldData = fd
        }

        this.errorMessage = errorMessage
      })
    )
  }

  terminalFeedObserver(fieldDataSetter: FieldDataSetter) : Observer<FieldData[][]> {
    return {
      next(fieldData : FieldData[][]) {
        fieldDataSetter(fieldData, undefined)
      },
      error(err: any) {
        fieldDataSetter(null, err.message)
        console.error(err)
      },
      complete() {}
    };
  }

  reset() {

  }

  enter() {
    this.wait = true
    this.sendUserData(Aid.ENTER)
  }

  functionKey(aid : Aid) {
    this.wait = true
    this.route66Service.sendKey(aid)?.then(fd => {
      this.fieldData = fd
      this.wait = false
    }, error => {
      this.errorMessage = error.message
      this.wait = false
    })
  }
  sendUserData(aid : Aid) {

    let fields : FieldData[] = []
    fields = fields.concat(...this.fieldData)

    const inputFields = fields.filter(f => !f.isProtected && f.dirty)
    const cursorField : FieldData | undefined = inputFields.find(f => {
      return this.cursor && this.cursor[0] == f.row && this.cursor[1] == f.col
    }) ?? inputFields.slice(-1)[0];

    (() => {
      if(this.cursor) {
        const cursorRow = this.cursor[0]
        const cursorCol = this.cursor[1] + (cursorField ? cursorField.value.length : 0);

        return this.route66Service.sendFields(aid, cursorRow, cursorCol, inputFields)
      } else {
        return this.route66Service.sendKey(aid)
      }
    })().then((fd : FieldData[][]) => {
      this.fieldData = fd
      this.wait = false
      this.errorMessage = undefined
    }, (error => {
      this.wait = false
      this.errorMessage = error.message
    }))
  }

  protected readonly Aid = Aid;
}

type FieldDataSetter = (fd: FieldData[][] | null, errorMessage: string | undefined) => void



