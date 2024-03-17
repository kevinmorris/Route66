import { Component, Input } from '@angular/core';
import { FieldData } from "../models/field-data";
import { NgStyle } from "@angular/common";

@Component({
  selector: 'app-input-field',
  standalone: true,
  imports: [
    NgStyle
  ],
  templateUrl: './input-field.component.html',
  styleUrl: './input-field.component.css'
})
export class InputFieldComponent {

  @Input({required: true}) fieldData!: FieldData;
}
