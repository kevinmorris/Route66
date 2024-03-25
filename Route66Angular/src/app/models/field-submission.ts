import { Aid } from "./aid";
import { FieldData } from "./field-data";

export type FieldSubmission = {
  aid : Aid;
  cursorRow : number;
  cursorCol : number;
  fieldData : FieldData[];
}
