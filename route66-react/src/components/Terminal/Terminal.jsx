import {useEffect, useState} from "react";
import {ReadyState} from "react-use-websocket";
import Constants from "../../Constants";
import Row from "../Row/Row";
import {inputValueChanged, processMessage, processRow} from "../../services/terminal_services";

export default function Terminal({websocket: {sendJsonMessage, lastJsonMessage, readyState}}) {

    const rowRange = Array.from({length: Constants.SCREEN_HEIGHT}, (_, i) => i)
    const [sessionKey, setSessionKey] = useState("");
    const [fieldData, setFieldData] = useState(rowRange.map(() => []));
    const [cursor, setCursor] = useState([-1, -1])

    console.info("XXXXXA256", lastJsonMessage)

    useEffect(() => {
        if(readyState === ReadyState.OPEN) {
            console.info("XXXXXA192")
            sendJsonMessage({
                instruction: 1,
                address: "127.0.0.1",
                port: "3270"
            })
        }
    }, [readyState])

    useEffect(() => {
        processMessage(lastJsonMessage, setSessionKey, processRow(setFieldData))
    }, [lastJsonMessage]);

    const rows = rowRange.map(i => <Row i={i}
                                        fieldData={fieldData[i]}
                                        inputChanged={inputValueChanged(setFieldData)}
                                        focusChanged={setCursor}></Row>)
    return (<div>{rows}</div>);
}
