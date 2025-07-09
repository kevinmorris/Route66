import Constants from "@/Constants.js";

export function processDisplayMessage(setFieldData) {
    return function(fieldData) {
        const rowRange = Array.from({length: Constants.SCREEN_HEIGHT}, (_, i) => i)
        const fields = rowRange.map(() => []);
        for (const fieldRow of fieldData) {
            if(fieldRow.length > 0) {
                const row = fieldRow[0].row;
                fields[row] = fieldRow;
            }
        }

        setFieldData(fields);
    }
}
