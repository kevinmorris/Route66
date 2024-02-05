let terminalDisplayObjRef = null;
function setDotNetObjRef(obj) {
    terminalDisplayObjRef = obj;
}

function tn3270inputChanged(row) {
    return terminalDisplayObjRef.invokeMethodAsync('InputChangedAsync', row);
}