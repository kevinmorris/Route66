import './App.css';
import Label from "./Label";
import Input from "./Input";
import useWebSocket, { ReadyState } from "react-use-websocket";
import Terminal from "./Terminal";


function App() {

  const {
    sendJsonMessage,
    lastJsonMessage,
    readyState
  } = useWebSocket("ws://127.0.0.1:7149/ws", {
    onError: (err) => {
      console.error("XXXXXA128", err)
    }
  });

  console.info("XXXXXA128", readyState)
  return <Terminal webSocket={{sendJsonMessage, lastJsonMessage, readyState}}/>
}

export default App;
