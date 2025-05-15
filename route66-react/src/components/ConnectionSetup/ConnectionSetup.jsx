import useWebSocket from "react-use-websocket";
import Terminal from "../Terminal/Terminal";
import {useState} from "react";

export default function ConnectionSetup() {

    const [shouldConnect, setShouldConnect] = useState(false)

    function connect() {
        setShouldConnect(true)
    }

    const {
        sendJsonMessage,
        lastJsonMessage,
        readyState
    } = useWebSocket( "ws://127.0.0.1:7149/ws",
        {
            onOpen: () => console.info("XXXXXA64"),
            onError: (err) => {
                console.error("XXXXXA128", err)
            },
        },
        shouldConnect);

    return (<>
        <div><button onClick={connect}>Connect</button></div>
        <Terminal websocket={{sendJsonMessage, lastJsonMessage, readyState}}/>
    </>)
}
