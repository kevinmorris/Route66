import { render, screen } from '@testing-library/react';
import LabelField from "./LabelField";

test('renders label field', () => {
    render(<LabelField fieldData={{row: 3, col: 5, value: "Alpha" }}/>);
    const labelElement = screen.getByTestId("3-5");
    expect(labelElement).toBeInTheDocument();
    expect(labelElement).toHaveTextContent("Alpha");
    expect(labelElement).toHaveStyle({left: '5ch'});
});
