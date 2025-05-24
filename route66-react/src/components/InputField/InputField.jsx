import styles from './InputField.module.css';

export default function InputField({fieldData, valueChanged, inputFocused }) {

    const style = {
        left: fieldData.col + 'ch',
        width: fieldData.length + 'ch',
    }

    return (
        <input
            style={style}
            value={fieldData.value}
            onInput={(event) => valueChanged(fieldData.row, fieldData.col, event.target.value)}
            onFocus={(event) => inputFocused(fieldData.row, fieldData.col)}
        />);

}
