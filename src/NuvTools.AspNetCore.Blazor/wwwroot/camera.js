class CameraInstance {

    constructor(video, canvas) {
        this._video = video;
        this._canvas = canvas;
        this._stream = null;
    }

    async start(options) {
        await this.stop();

        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia)
            return { name: "NotSupportedError", message: "getUserMedia is not available in this browser." };

        const constraints = {};
        if (options.width) constraints.width = { ideal: options.width };
        if (options.height) constraints.height = { ideal: options.height };
        if (options.deviceId) constraints.deviceId = { exact: options.deviceId };
        else if (options.facingMode) constraints.facingMode = options.facingMode;

        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                video: Object.keys(constraints).length ? constraints : true,
                audio: false
            });

            this._stream = stream;

            const video = this._video;
            video.srcObject = stream;
            video.autoplay = true;
            video.muted = true;
            // iOS Safari only honours the attribute; without it the video opens fullscreen.
            video.playsInline = true;
            video.setAttribute("playsinline", "");
            video.style.transform = options.mirrored ? "scaleX(-1)" : "";

            // Autoplay may be blocked by the browser; the stream stays live either way.
            await video.play().catch(() => { });

            return null;
        } catch (error) {
            await this.stop();
            return { name: error?.name ?? "Error", message: error?.message ?? String(error) };
        }
    }

    async stop() {
        if (this._stream) {
            this._stream.getTracks().forEach(track => track.stop());
            this._stream = null;
        }

        const video = this._video;
        if (video) {
            try { video.pause(); } catch { /* not playing */ }
            video.srcObject = null;
        }
    }

    capture(options) {
        const video = this._video;
        if (!video || !video.videoWidth || !video.videoHeight) return null;

        const width = options.width || video.videoWidth;
        const height = options.height || video.videoHeight;

        const canvas = this._canvas;
        canvas.width = width;
        canvas.height = height;

        const context = canvas.getContext("2d");
        context.save();

        if (options.mirrored) {
            context.scale(-1, 1);
            context.drawImage(video, -width, 0, width, height);
        } else {
            context.drawImage(video, 0, 0, width, height);
        }

        context.restore();

        return {
            dataUrl: canvas.toDataURL(options.contentType, options.quality),
            width: width,
            height: height
        };
    }

    dispose() {
        this.stop();
        this._video = null;
        this._canvas = null;
    }
}

export function create(video, canvas) {
    return new CameraInstance(video, canvas);
}

export async function listDevices() {
    if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) return [];

    const devices = await navigator.mediaDevices.enumerateDevices();

    // Labels are only populated once the user has granted camera permission at least once.
    return devices
        .filter(device => device.kind === "videoinput")
        .map(device => ({ deviceId: device.deviceId, label: device.label }));
}
