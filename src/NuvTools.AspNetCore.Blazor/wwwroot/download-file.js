const _chunks = [];

function triggerDownload(blob, fileName) {
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName ?? "";
    anchor.click();
    anchor.remove();
    URL.revokeObjectURL(url);
}

export function downloadBase64(fileName, base64Data, contentType) {
    const raw = atob(base64Data);
    const bytes = new Uint8Array(raw.length);
    for (let i = 0; i < raw.length; i++) {
        bytes[i] = raw.charCodeAt(i);
    }
    const blob = new Blob([bytes], { type: contentType });
    triggerDownload(blob, fileName);
}

export function addChunk(base64Chunk) {
    _chunks.push(base64Chunk);
}

export function downloadFromChunks(fileName, contentType) {
    const joined = _chunks.join("");
    _chunks.length = 0;
    const raw = atob(joined);
    const bytes = new Uint8Array(raw.length);
    for (let i = 0; i < raw.length; i++) {
        bytes[i] = raw.charCodeAt(i);
    }
    const blob = new Blob([bytes], { type: contentType });
    triggerDownload(blob, fileName);
}
