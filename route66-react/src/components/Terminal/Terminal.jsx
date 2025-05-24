import {useEffect, useState} from "react";
import {ReadyState} from "react-use-websocket";
import WebSocketInstruction from "../../WebSocketInstruction";
import Constants from "../../Constants";
import Row from "../Row/Row";

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
        if (!lastJsonMessage) return;

        if (lastJsonMessage.instruction === WebSocketInstruction.STARTING_CONNECTION) {
            setSessionKey(lastJsonMessage.sessionKey);
        } else if (lastJsonMessage.instruction === WebSocketInstruction.ROW) {
            processRow(lastJsonMessage, setFieldData);
        }
    }, [lastJsonMessage]);

    function processRow(message, setData) {

        const index = message.row;

        setData(prevFields => {
            return prevFields.map((field, i) => i === index ? message.fieldData : field);
        })
    }

    function inputValueChanged(row, col, value) {
        setFieldData(prevFields => {
            const targetRow = fieldData[row];
            const updatedRow = targetRow.map(f => {
                return f.col === col ? {...f, dirty: true, value} : f
            })

            const updatedFields = [
                ...prevFields.slice(0, row),
                updatedRow,
                ...prevFields.slice(row + 1)
            ];

            return updatedFields;
        });
    }

    const rows = rowRange.map(i => <Row i={i}
                                        fieldData={fieldData[i]}
                                        inputChanged={inputValueChanged}
                                        focusChanged={setCursor}></Row>)
    return (<div>{rows}</div>);
}
