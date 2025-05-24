import LabelField from "../LabelField/LabelField";
import styles from './Row.module.css';
import InputField from "../InputField/InputField";


export default function Row({ i, fieldData, inputChanged, focusChanged })  {

    const fields = fieldData.map((field, i) => {
        if(field.isProtected) {
            return <LabelField key={`${i}-${field.col}`} fieldData={field}></LabelField>
        } else {
            return <InputField key={`${i}-${field.col}`}
                               fieldData={field}
                               valueChanged={inputChanged}
                               inputFocused={focusChanged}>
                   </InputField>
        }
    })

    return (<div className={styles.row}>{fields}</div>)
}
