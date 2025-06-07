import {useEffect, useState} from "react";
import {ReadyState} from "react-use-websocket";
import Constants from "../../Constants";
import Row from "../Row/Row";
import {createFieldSubmission, inputValueChanged, processMessage, processRow} from "../../services/terminal_services";

export default function Terminal({websocket: {sendJsonMessage, lastJsonMessage, readyState}}) {

    const rowRange = Array.from({length: Constants.SCREEN_HEIGHT}, (_, i) => i)
    const [sessionKey, setSessionKey] = useState("");
    const [fieldData, setFieldData] = useState(rowRange.map(() => []));
    const [cursor, setCursor] = useState([-1, -1])

    useEffect(() => {
        if(readyState === ReadyState.OPEN) {
            sendJsonMessage({
                instruction: 1,
                address: "127.0.0.1",
                port: "3270"
            })
        }
    }, [readyState])

    useEffect(() => {
        window.addEventListener('keydown', handleKeyDown)
        return () => window.removeEventListener('keydown', handleKeyDown)
    })

    useEffect(() => {
        processMessage(lastJsonMessage, setSessionKey, processRow(setFieldData))
    }, [lastJsonMessage]);

    function handleKeyDown(event) {
        if(event.key === 'Enter') {
            enterKey();
        }
    }

    function functionKey(key) {
        const body = {
            instruction: Constants.WEB_SOCKET_INSTRUCTION.SUBMIT_FIELDS,
            sessionKey: sessionKey,
            submission: {
                aid: key,
            }
        }

        sendJsonMessage(body)
    }

    function enterKey() {
        const fieldSubmission = createFieldSubmission(cursor, fieldData)
        const body = {
            instruction: Constants.WEB_SOCKET_INSTRUCTION.SUBMIT_FIELDS,
            sessionKey: sessionKey,
            submission: {
                aid: Constants.AID.ENTER,
                ...fieldSubmission
            }
        }

        sendJsonMessage(body)
    }

    const rows = rowRange.map(i => <Row i={i}
                                        fieldData={fieldData[i]}
                                        inputChanged={inputValueChanged(setFieldData)}
                                        focusChanged={setCursor}></Row>)

    return (
        <div id="container" className="border-zero">
            <div>
                <button className="keypad-button">Reset</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PA1)}>PA1</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PA2)}>PA2</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PA3)}>PA3</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.SYSREQ)}>Sys Req</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.CLEAR)}>Clear</button>
            </div>
            <div>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF1)}>PF1</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF2)}>PF2</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF3)}>PF3</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF4)}>PF4</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF5)}>PF5</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF6)}>PF6</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF7)}>PF7</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF8)}>PF8</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF9)}>PF9</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF10)}>PF10</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF11)}>PF11</button>
                <button className="keypad-button" onClick={() => functionKey(Constants.AID.PF12)}>PF12</button>
            </div>
            <div>{rows}</div>
        </div>);
}
