import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FieldData } from "../models/field-data";
import { NgForOf, NgIf } from "@angular/common";
import { LabelFieldComponent } from "../label-field/label-field.component";
import { InputFieldComponent } from "../input-field/input-field.component";

@Component({
  selector: 'app-row',
  standalone: true,
  imports: [
    NgIf,
    NgForOf,
    LabelFieldComponent,
    InputFieldComponent
  ],
  templateUrl: './row.component.html',
  styleUrl: './row.component.css'
})
export class RowComponent {

  @Input({required: true}) index! : number;
  @Input() fieldData!: FieldData[]

  @Output() focused = new EventEmitter<[number, number]>

  onInputFocus(event : [number, number]) {
    this.focused.emit(event)
  }
}
