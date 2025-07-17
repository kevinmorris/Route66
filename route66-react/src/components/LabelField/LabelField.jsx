import styles from './LabelField.module.css';

export default function LabelField({ fieldData }) {

    const style = {
        "left": fieldData.col + 'ch'
    };

    const testId = `${fieldData.row}-${fieldData.col}`

    return (
        <span className={styles.field}
              style={style}
              data-testid={testId}>
            {fieldData.value}
        </span>
    );
}
