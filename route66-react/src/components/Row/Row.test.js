import { render } from '@testing-library/react';
import Row from "./Row";
import LabelField from "../LabelField/LabelField";
import InputField from "../InputField/InputField";

// Mock the child component as a Jest mock function
jest.mock('../InputField/InputField', () => ({
    __esModule: true,
    default: jest.fn(() => <div data-testid="mock-input" />)
}));
jest.mock('../LabelField/LabelField', () => ({
    __esModule: true,
    default: jest.fn(() => <div data-testid="mock-label" />)
}));

test('calls Input field and Label field', () => {

    const fieldData = [
        {
            "row": 22,
            "col": 0,
            "value": "Logon ===>",
            "isProtected": true,
            "address": 1760,
            "length": 10,
            "dirty": false,
            "cursor": -1
        },
        {
            "row": 22,
            "col": 11,
            "value": "                                                                     ",
            "isProtected": false,
            "address": 1771,
            "length": 69,
            "dirty": false,
            "cursor": 11
        }
    ];

    render(<Row key={22} i={22} fieldData={fieldData} />);

    expect(LabelField).toHaveBeenCalledTimes(1)
    expect(InputField).toHaveBeenCalledTimes(1)

    expect(LabelField).toHaveBeenCalledWith(
        expect.objectContaining({
            fieldData: fieldData[0],
        }),
        {}
    );

    expect(InputField).toHaveBeenCalledWith(
        expect.objectContaining({
            fieldData: fieldData[1],
        }),
        {}
    );
});
