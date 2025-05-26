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
    return function(message) {
        const index = message.row;

        setFieldData(prevFields => {
            return prevFields.map((field, i) => i === index ? message.fieldData : field);
        })
    }
}
