const WebSocketInstruction = Object.freeze({
    OK: 0,
    CONNECT: 1,
    STARTING_CONNECTION: 2,
    DISPLAY: 3,
    SUBMIT_FIELDS: 4,
    ERROR: 5
});

export default WebSocketInstruction;
