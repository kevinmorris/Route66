const WebSocketInstruction = Object.freeze({
    OK: 0,
    CONNECT: 1,
    STARTING_CONNECTION: 2,
    DISPLAY: 3,
    ROW: 4,
    SUBMIT_FIELDS: 5,
    ERROR: 6
});

export default WebSocketInstruction;
