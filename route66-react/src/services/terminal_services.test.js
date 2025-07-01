import {createFieldSubmission, inputValueChanged, processDisplayMessage} from "./terminal_services";

describe('inputValueChanged', () => {
    test('mark field as dirty', () => {
        const prevFields = [
            [],
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
                    "value": "                                                                                                                                ",
                    "isProtected": false,
                    "address": 1771,
                    "length": 128,
                    "dirty": false,
                    "cursor": 1771
                }
            ],
            [
                {
                    "row": 2,
                    "col": 60,
                    "value": "RUNNING  TK5",
                    "isProtected": true,
                    "address": 1900,
                    "length": 12,
                    "dirty": false,
                    "cursor": -1
                }
            ]
        ];

        const expected = [
            [],
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
                    "value": "alpha1",
                    "isProtected": false,
                    "address": 1771,
                    "length": 128,
                    "dirty": true,
                    "cursor": 1771
                }
            ],
            [
                {
                    "row": 2,
                    "col": 60,
                    "value": "RUNNING  TK5",
                    "isProtected": true,
                    "address": 1900,
                    "length": 12,
                    "dirty": false,
                    "cursor": -1
                }
            ]
        ];
        const mockSetFieldData = jest.fn();
        const inputValueChangedFn = inputValueChanged(mockSetFieldData);
        inputValueChangedFn(1, 11, "alpha1")

        const updaterFn = mockSetFieldData.mock.calls[0][0];
        const actual = updaterFn(prevFields);
        expect(actual).toEqual(expected);
    });
});

describe('processDisplayMessage', () => {
    test('updates the rows', () => {
        const initialFieldData = [
            [
                {
                    "row": 0,
                    "col": 0,
                    "value": "Terminal",
                    "isProtected": true,
                    "address": -1,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 0,
                    "col": 11,
                    "value": "CUU0C0  ",
                    "isProtected": true,
                    "address": 11,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 0,
                    "col": 66,
                    "value": "Date",
                    "isProtected": true,
                    "address": 49218,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 0,
                    "col": 72,
                    "value": "23.06.25",
                    "isProtected": true,
                    "address": 49224,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 1,
                    "col": 66,
                    "value": "Time",
                    "isProtected": true,
                    "address": 49298,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 1,
                    "col": 72,
                    "value": "18:38:30",
                    "isProtected": true,
                    "address": 49304,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 3,
                    "col": 27,
                    "value": "TTTTTTTTTTTT",
                    "isProtected": true,
                    "address": 33035,
                    "length": 12,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 42,
                    "value": "KKKK",
                    "isProtected": true,
                    "address": 49434,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 48,
                    "value": "KKKKK",
                    "isProtected": true,
                    "address": 33056,
                    "length": 5,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 57,
                    "value": "555555555555",
                    "isProtected": true,
                    "address": 49449,
                    "length": 12,
                    "dirty": false,
                    "cursor": -1
                }
            ]
        ];

        const expectedFields = [
            [
                {
                    "row": 0,
                    "col": 0,
                    "value": "Terminal",
                    "isProtected": true,
                    "address": -1,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 0,
                    "col": 11,
                    "value": "CUU0C0  ",
                    "isProtected": true,
                    "address": 11,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 0,
                    "col": 66,
                    "value": "Date",
                    "isProtected": true,
                    "address": 49218,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 0,
                    "col": 72,
                    "value": "23.06.25",
                    "isProtected": true,
                    "address": 49224,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [
                {
                    "row": 1,
                    "col": 66,
                    "value": "Time",
                    "isProtected": true,
                    "address": 49298,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 1,
                    "col": 72,
                    "value": "18:38:30",
                    "isProtected": true,
                    "address": 49304,
                    "length": 8,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [],
            [
                {
                    "row": 3,
                    "col": 27,
                    "value": "TTTTTTTTTTTT",
                    "isProtected": true,
                    "address": 33035,
                    "length": 12,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 42,
                    "value": "KKKK",
                    "isProtected": true,
                    "address": 49434,
                    "length": 4,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 48,
                    "value": "KKKKK",
                    "isProtected": true,
                    "address": 33056,
                    "length": 5,
                    "dirty": false,
                    "cursor": -1
                },
                {
                    "row": 3,
                    "col": 57,
                    "value": "555555555555",
                    "isProtected": true,
                    "address": 49449,
                    "length": 12,
                    "dirty": false,
                    "cursor": -1
                }
            ],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
    ];

        const mockSetFieldData = jest.fn()

        const displayMessageFn = processDisplayMessage(mockSetFieldData);
        displayMessageFn(initialFieldData);

        const actualFields = mockSetFieldData.mock.calls[0][0];
        expect(actualFields).toEqual(expectedFields)
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
