import { Aid } from "./aid";
import { FieldData } from "./field-data";

export type FieldSubmission = {
  aid : Aid;
  cursorRow : number | undefined;
  cursorCol : number | undefined;
  fieldData : FieldData[] | undefined;
}
