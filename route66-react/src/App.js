import './App.css';
import Label from "./Label";
import Input from "./Input";
import {useState} from "react";

function App() {

  const [fieldData, setFieldData] = useState({ col: 4, value: "HERC01"});
  const onValueChange = (value) => {
    console.info("XXXXXA128", value);
    setFieldData({ ...fieldData, value })
  }

  return (
    <Input fieldData={fieldData} valueChanged={onValueChange}/>
  );
}

export default App;
