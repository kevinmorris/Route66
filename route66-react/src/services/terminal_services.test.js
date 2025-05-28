import {createFieldSubmission, processRow} from "./terminal_services";

describe('processRow', () => {
    test('updates the correct row', () => {
        const initialFieldData = [
            { value: 'row0', dirty: false },
            { value: 'row1', dirty: false },
            { value: 'row2', dirty: false }
        ];

        const mockSetFieldData = jest.fn()
        const rowMessage = {
            row: 1,
            fieldData: { value: 'Alpha', dirty: true }
        };

        const updateRow = processRow(mockSetFieldData);
        updateRow(rowMessage);

        const updaterFn = mockSetFieldData.mock.calls[0][0]
        const result = updaterFn(initialFieldData)
        expect(result).toEqual([
            { value: 'row0', dirty: false },
            { value: 'Alpha', dirty: true },
            { value: 'row2', dirty: false }
        ])
    });
});

describe('createFieldSubmission', () => {
    test('dirty fields', () => {
        const cursor = [2, 4];
        const fieldData = [
            [
                {
                    "row": 1,
                    "col": 0,
                    "value": "Logon ===>",
                    "isProtected": true,
                    "address": 1760,
                    "length": 10,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 1,
                    "col": 11,
                    "value": "                                                                     ",
                    "isProtected": false,
                    "address": 91,
                    "length": 69,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 2,
                    "col": 4,
                    "value": "Alpha",
                    "isProtected": false,
                    "address": 164,
                    "length": 5,
                    "dirty": true,
                    "cursor": -1
                },
                {
                    "row": 2,
                    "col": 25,
                    "value": "Bravo             ",
                    "isProtected": true,
                    "address": 205,
                    "length": 18,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 3,
                    "col": 6,
                    "value": "Charlie",
                    "isProtected": true,
                    "address": 246,
                    "length": 7,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 20,
                    "value": "Delta",
                    "isProtected": false,
                    "address": 260,
                    "length": 5,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 40,
                    "value": "Echo",
                    "isProtected": false,
                    "address": 280,
                    "length": 4,
                    "dirty": true,
                    "cursor": -1
                }
            ]
        ];

        const submission = createFieldSubmission(cursor, fieldData);
        expect(submission).toEqual({
            cursorRow: 2,
            cursorCol: 9,
            fieldData: [
                {
                    "row": 2,
                    "col": 4,
                    "value": "Alpha",
                    "address": 164,
                },
                {
                    "row": 3,
                    "col": 40,
                    "value": "Echo",
                    "address": 280,
                }
            ]
        })
    })

    test('clean fields', () => {
        const cursor = [2, 4];
        const fieldData = [
            [
                {
                    "row": 1,
                    "col": 0,
                    "value": "Logon ===>",
                    "isProtected": true,
                    "address": 1760,
                    "length": 10,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 1,
                    "col": 11,
                    "value": "                                                                     ",
                    "isProtected": false,
                    "address": 91,
                    "length": 69,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 2,
                    "col": 4,
                    "value": "Alpha",
                    "isProtected": false,
                    "address": 164,
                    "length": 5,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 2,
                    "col": 25,
                    "value": "Bravo             ",
                    "isProtected": true,
                    "address": 205,
                    "length": 18,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 3,
                    "col": 6,
                    "value": "Charlie",
                    "isProtected": true,
                    "address": 246,
                    "length": 7,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 20,
                    "value": "Delta",
                    "isProtected": false,
                    "address": 260,
                    "length": 5,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 40,
                    "value": "Echo",
                    "isProtected": false,
                    "address": 280,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                }
            ]
        ];

        const submission = createFieldSubmission(cursor, fieldData);
        expect(submission).toEqual({
            cursorRow: 2,
            cursorCol: 4,
            fieldData: []
        })
    })
});
