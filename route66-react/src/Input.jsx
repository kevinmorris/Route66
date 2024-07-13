import styles from './css/Input.module.css';

export default function Input({fieldData, valueChanged }) {

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