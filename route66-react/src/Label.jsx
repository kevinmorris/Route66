import styles from './css/Label.module.css';

export default function Label({ fieldData }) {

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