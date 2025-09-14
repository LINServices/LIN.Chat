// Mantiene una única instancia de Audio para controlar reproducir/pausar
let audioInstance = null;

export function playAudio(src) {
    try {
        if (!audioInstance) {
            audioInstance = new Audio();
            audioInstance.loop = true; // que suene en bucle mientras está abierto
        }
        if (audioInstance.src !== new URL(src, document.baseURI).href) {
            audioInstance.src = src;
        }
        audioInstance.currentTime = 0;
        // play() devuelve una Promise; ignoramos el error de autoplay si el usuario aún no interactuó
        audioInstance.play().catch(() => { });
    } catch (e) {
        console.warn("playAudio error", e);
    }
}

export function stopAudio() {
    try {
        if (audioInstance) {
            audioInstance.pause();
            audioInstance.currentTime = 0;
        }
    } catch (e) {
        console.warn("stopAudio error", e);
    }
}
