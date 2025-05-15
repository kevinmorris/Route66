import LabelField from "../LabelField/LabelField";
import styles from './Row.module.css';


export default function Row({ i, fieldData })  {

    const fields = fieldData.map((field, i) => <LabelField row={i} fieldData={field}></LabelField> )
    return (<div className={styles.row}>
        {fields}
    </div>)
}
