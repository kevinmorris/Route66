import {useEffect, useState} from "react";
import {ReadyState} from "react-use-websocket";

export default function Terminal({ webSocket: { sendJsonMessage, lastJsonMessage, readyState } }) {
    const [fieldData, setFieldData] = useState([[{}]]);

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

    console.info("XXXXXA256", lastJsonMessage)
}