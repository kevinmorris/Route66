import styles from './css/InputField.module.css';

export default function InputField({fieldData, valueChanged }) {

    const style = {
        left: fieldData.col + 'ch',
        width: fieldData.length + 'ch',
    }

    return (
        <input
            style={style}
            value={fieldData.value}
            onInput={(event) => valueChanged(event.target.value)}
        />);

}
