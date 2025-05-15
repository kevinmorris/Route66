import styles from './css/LabelField.module.css';

export default function LabelField({ fieldData }) {

    const style = {
        "left": fieldData.col + 'ch'
    };

    return (
        <span className={styles.field}
              style={style}>
            {fieldData.value}
        </span>
    );
}
