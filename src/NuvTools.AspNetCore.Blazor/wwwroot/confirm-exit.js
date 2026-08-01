let _attached = false;

function onBeforeUnload(event) {
    event.preventDefault();
    // The warning text is defined by the browser; custom messages are ignored.
    event.returnValue = "";
    return "";
}

export function enable() {
    if (_attached) return;
    window.addEventListener("beforeunload", onBeforeUnload);
    _attached = true;
}

export function disable() {
    if (!_attached) return;
    window.removeEventListener("beforeunload", onBeforeUnload);
    _attached = false;
}

export function isEnabled() {
    return _attached;
}
