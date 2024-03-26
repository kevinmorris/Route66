import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FieldData } from "../models/field-data";
import { NgStyle } from "@angular/common";
import { FormsModule } from "@angular/forms";

@Component({
  selector: 'app-input-field',
  standalone: true,
  imports: [
    NgStyle,
    FormsModule
  ],
  templateUrl: './input-field.component.html',
  styleUrl: './input-field.component.css'
})
export class InputFieldComponent {

  @Input({required: true}) fieldData!: FieldData;
  @Output() focused = new EventEmitter<[number, number]>();

  onFocus() {
    this.fieldData.value = this.fieldData.value.trim()
    this.focused.emit([this.fieldData.row, this.fieldData.col])
  }

  valueChanged(value : string) {
    this.fieldData.value = value.trim()
  }
}
