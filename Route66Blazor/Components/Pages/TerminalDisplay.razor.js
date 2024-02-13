let terminalDisplayObjRef = null;
let focusCoords = [-1, -1]

function onFocusChanged(field) {
    focusCoords = [
        field.dataset.row,
        field.dataset.col
    ]
}

function setDotNetObjRef(obj) {
    terminalDisplayObjRef = obj;
}

function tn3270inputChanged(row) {
    return terminalDisplayObjRef.invokeMethodAsync('InputChangedAsync', row);
}

function assembleInputFields() {

    const c = document.getElementById('container');
    const inputFields = Array.from(
        c.querySelectorAll('input[data-row][data-col]'));
    const content = inputFields.map((field) => {
        return {
            row: field.dataset.row,
            col: field.dataset.col,
            value: field.value,

        }});

    return {
        cursorRow: focusCoords[0],
        cursorCol: focusCoords[1],
        fields: content
    };
}