import Constants from "@/Constants.js";
import {gql} from "@apollo/client/core";

export const gqlConstants = {
    connect: gql(`mutation Connect($address: String!, $port: Int!) {
        connect(connectRequest:  {
            address: $address
            port: $port
        }) {
            sessionKey
            address
            port
        }
    }`),

    displayQuery: gql`query($sessionKey: String!) {
        display(sessionKey: $sessionKey) {
            fieldData {
                row
                col
                value
                isProtected
                address
                length
                dirty
                cursor
            }
        }
    }`,

    displaySubscription: gql`subscription display($sessionKey: String!) {
        display(sessionKey: $sessionKey) {
            fieldData {
                row
                col
                value
                isProtected
                address
                length
                dirty
                cursor
            }
        }
    }`,

    submitFields: gql`mutation SubmitFields($submission: Submission!) {
        submitFields(submission: $submission) {
            code
        }
    }`,
}

export function processDisplayMessage(fieldData) {
    const rowRange = Array.from({length: Constants.SCREEN_HEIGHT}, (_, i) => i)
    const fields = rowRange.map(() => []);
    for (const fieldRow of fieldData) {
        if(fieldRow.length > 0) {
            const row = fieldRow[0].row;
            fields[row] = fieldRow;
        }
    }

    return fields;
}

export function inputValueChanged(prevFields, row, col, value) {
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
