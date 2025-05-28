import WebSocketInstruction from "../WebSocketInstruction";

export function inputValueChanged(setFieldData) {
    return function(row, col, value) {
        setFieldData(prevFields => {
            const targetRow = prevFields[row];
            const updatedRow = targetRow.map(f => {
                return f.col === col ? {...f, dirty: true, value} : f
            })

            const updatedFields = [
                ...prevFields.slice(0, row),
                updatedRow,
                ...prevFields.slice(row + 1)
            ];

            return updatedFields;
        });
    }
}

export function processMessage(lastJsonMessage, setSessionKey, processRow) {
    if (!lastJsonMessage) return;

    if (lastJsonMessage.instruction === WebSocketInstruction.STARTING_CONNECTION) {
        setSessionKey(lastJsonMessage.sessionKey);
    } else if (lastJsonMessage.instruction === WebSocketInstruction.ROW) {
        processRow(lastJsonMessage);
    }
}

export function processRow(setFieldData) {
    return function(rowMessage) {
        const index = rowMessage.row;

        setFieldData(prevFields => {
            return prevFields.map((field, i) => i === index ? rowMessage.fieldData : field);
        })
    }
}

export function createFieldSubmission(cursor, fieldData) {

    const dirtyFields = fieldData.flatMap(row => {
        return row.filter(field => field.dirty && !field.isProtected)
    });

    const strippedFields = dirtyFields.map(field => {
        return Object.fromEntries(Object.entries(field).filter(([key]) => {
            return [ 'row', 'col', 'value', 'address' ].includes(key)
        }))
    });

    const cursorField = strippedFields.find(f => f.row === cursor[0] && f.col === cursor[1]) ||
        strippedFields.at(-1);

    const cursorSubmission = {
        cursorRow: cursor[0],
        cursorCol: cursorField?.col != null ?
            (cursorField.col + cursorField.value.length)
            : cursor[1]
    };

    return {
        ...cursorSubmission,
        fieldData: strippedFields
    };
}
