import { Component, Input } from '@angular/core';
import { FieldData } from "../models/field-data";
import { NgStyle } from "@angular/common";

@Component({
  selector: 'app-label-field',
  standalone: true,
  imports: [
    NgStyle
  ],
  templateUrl: './label-field.component.html',
  styleUrl: './label-field.component.css'
})
export class LabelFieldComponent {

  @Input({required: true}) fieldData!: FieldData;
}
