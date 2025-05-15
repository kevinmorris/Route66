import LabelField from "./LabelField";
import styles from './css/Row.module.css';


export default function Row({ i, fieldData })  {

    const fields = fieldData.map((field, i) => <LabelField fieldData={field}></LabelField> )
    return (<div className={styles.row}>
        {fields}
    </div>)
}
