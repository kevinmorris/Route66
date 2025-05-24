import {render, screen, fireEvent} from '@testing-library/react';
import InputField from "./InputField";

test('renders input field', () => {
    const mockValueChanged = jest.fn();
    render(<InputField fieldData={{row: 3, col: 5, length: 10, value: "Bravo"}} valueChanged={mockValueChanged}/>);

    const inputElement = screen.getByRole('textbox');
    expect(inputElement).toBeInTheDocument();
    expect(inputElement).toHaveValue("Bravo");
    expect(inputElement).toHaveStyle({
        left: '5ch',
        width: '10ch'
    });
});

test('handles value changes', () => {
    const mockValueChanged = jest.fn();
    render(<InputField fieldData={{row: 3, col: 5, length: 10, value: "Charlie"}} valueChanged={mockValueChanged}/>);

    const inputElement = screen.getByRole('textbox');
    fireEvent.input(inputElement, {target: {value: 'Delta'}});

    expect(mockValueChanged).toHaveBeenCalledWith(3, 5, 'Delta');
});
