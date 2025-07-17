export default function InputField({fieldData, valueChanged, inputFocused }) {

    const style = {
        left: fieldData.col + 'ch',
        width: fieldData.length + 'ch',
    }

    return (
        <input
            style={style}
            value={fieldData.value && fieldData.value.trim().length === 0 ? "" : fieldData.value}
            onInput={(event) => valueChanged(fieldData.row, fieldData.col, event.target.value)}
            onFocus={(event) => inputFocused([fieldData.row, fieldData.col])}
        />);

}
