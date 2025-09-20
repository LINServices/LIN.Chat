// webrtc.js (solo lo clave)
window.webrtc = (function () {
    let connection = null;
    let localStream = null;
    let pcs = new Map(); // targetId -> { pc, remoteStream }
    let myId = null;
    let roomId = null;

    const servers = {
        iceServers: [
            { urls: "stun:stun.l.google.com:19302" }
            // Agrega TURN aquí para fuera de LAN (ver sección TURN más abajo)
        ]
    };

    const els = { grid: "videoGrid", localVideo: "localVideo" };
    const byId = id => document.getElementById(id);

    async function init(hubUrl) {
        const signalR = window.signalR;
        connection = new signalR.HubConnectionBuilder().withUrl(hubUrl).withAutomaticReconnect().build();

        connection.on("PeersInRoom", async (ids) => {
            // Crea offers a los que ya estaban
            for (const id of ids)
                await ensurePeerAndOffer(id);
        });

        connection.on("PeerJoined", async (id) => {
            // opcional: el que entra te hará offer; puedes no hacer nada aquí
            console.log("PeerJoined", id);
        });

        connection.on("PeerLeft", (id) => {
            const it = pcs.get(id);
            console.log("PeerLeft", id);
            if (it) {
                it.pc.close();
                pcs.delete(id);
                removeVideoEl(id);
            }
        });

        connection.on("Sdp", async (fromId, type, sdp) => {
            const it = await ensurePeer(fromId);
            await it.pc.setRemoteDescription({ type, sdp });
            if (type === "offer") {
                const answer = await it.pc.createAnswer();
                await it.pc.setLocalDescription(answer);
                await connection.invoke("SendSdp", fromId, "answer", answer.sdp);
            }
        });

        connection.on("Ice", async (fromId, candidate, sdpMid, sdpMLineIndex) => {
            const it = pcs.get(fromId);
            if (it && candidate) {
                try { await it.pc.addIceCandidate({ candidate, sdpMid, sdpMLineIndex }); } catch (e) { console.warn(e); }
            }
        });

        await connection.start();
        console.log("SignalR connected");
    }

    async function join(rid, token) {
        requestWakeLock();
        roomId = rid;
        await getMedia();
        console.log("Joining")
        await connection.invoke("Join", rid, token);
        console.log("Joined")
    }

    async function getMedia() {
        if (localStream) return localStream;
        localStream = await navigator.mediaDevices.getUserMedia({
            audio: { echoCancellation: true, noiseSuppression: true, autoGainControl: true },
            video: { width: { ideal: 1280 }, height: { ideal: 720 } }
        });
        const lv = byId(els.localVideo);
        if (lv) { lv.srcObject = localStream; lv.muted = true; lv.playsInline = true; try { await lv.play(); } catch { } }
        return localStream;
    }

    function makePc(targetId) {
        const pc = new RTCPeerConnection(servers);
        const remoteStream = new MediaStream();

        pc.onicecandidate = async (ev) => {
            if (ev.candidate) {
                await connection.invoke("SendIce", targetId, ev.candidate.candidate, ev.candidate.sdpMid, ev.candidate.sdpMLineIndex);
            }
        };

        pc.ontrack = (ev) => {
            ev.streams[0].getTracks().forEach(t => {
                if (!remoteStream.getTracks().some(x => x.id === t.id)) remoteStream.addTrack(t);
            });
            attachRemote(targetId, remoteStream);
        };

        // simulcast opcional (mejor con SFU, pero lo dejamos simple)
        const stream = localStream;
        stream.getTracks().forEach(t => pc.addTrack(t, stream));

        return { pc, remoteStream };
    }

    async function ensurePeer(targetId) {
        if (pcs.has(targetId)) return pcs.get(targetId);
        const it = makePc(targetId);
        pcs.set(targetId, it);
        createVideoEl(targetId); // crea <video id="peer-{id}">
        return it;
    }

    async function ensurePeerAndOffer(targetId) {
        const it = await ensurePeer(targetId);
        const offer = await it.pc.createOffer({ offerToReceiveAudio: true, offerToReceiveVideo: true });
        await it.pc.setLocalDescription(offer);
        await connection.invoke("SendSdp", targetId, "offer", offer.sdp);
    }

    function createVideoEl(id) {
        const grid = document.getElementById("grid-llamada");
        if (!grid) return;

        // Contenedor para aplicar estilos de Tailwind
        const wrapper = document.createElement("div");
        wrapper.id = `ct-${id}`;
        wrapper.className = "relative aspect-video bg-gray-800 rounded-2xl overflow-hidden shadow-lg";

        // El elemento video
        const v = document.createElement("video");
        v.id = `peer-${id}`;
        v.autoplay = true;
        v.playsInline = true;
        v.setAttribute("data-peer", id);
        v.className = "w-full h-full object-cover"; // Para que se ajuste al contenedor

        // Añadimos el video al wrapper
        wrapper.appendChild(v);

        // Añadimos el wrapper al grid
        grid.appendChild(wrapper);

        return v; // opcional, por si necesitas manipular el video luego
    }


    function removeVideoEl(id) {
        let v = byId(`peer-${id}`);
        if (v && v.parentNode)
            v.parentNode.removeChild(v);

        v = byId(`ct-${id}`);
        if (v && v.parentNode)
            v.parentNode.removeChild(v);
    }

    async function attachRemote(id, stream) {
        const v = byId(`peer-${id}`);
        if (!v) return;
        v.srcObject = stream;
        try { await v.play(); } catch { }
    }

    async function hangup() {
        for (const [, it] of pcs) {
            try { it.pc.getSenders().forEach(s => s.track?.stop()); it.pc.close(); } catch { }
        }
        pcs.clear();
        if (localStream) { localStream.getTracks().forEach(t => t.stop()); localStream = null; }
        const grid = byId(els.grid); if (grid) grid.innerHTML = "";
        await connection.invoke("Leave");
    }

    //  return { init, join, hangup, setEls: (m) => Object.assign(els, m) };

    // Estado extra
    let cameraStream = null;
    let screenStream = null;
    let usingScreen = false;

    // Reemplaza la pista saliente de VIDEO en TODOS los peers
    async function replaceVideoTrackForAllPeers(track) {
        for (const [, it] of pcs) {
            const sender = it.pc.getSenders().find(s => s.track && s.track.kind === "video");
            if (sender) await sender.replaceTrack(track);
        }
    }

    // Sobrescribe getMedia para separar cámara de flujo "actual"
    async function getMedia() {
        if (localStream) return localStream;
        cameraStream = await navigator.mediaDevices.getUserMedia({
            audio: { echoCancellation: true, noiseSuppression: true, autoGainControl: true },
            video: { width: { ideal: 1280 }, height: { ideal: 720 } }
        });
        localStream = cameraStream;

        const lv = byId(els.localVideo);
        if (lv) { lv.srcObject = localStream; lv.muted = true; lv.playsInline = true; try { await lv.play(); } catch { } }
        return localStream;
    }

    // Iniciar compartir pantalla
    async function startScreenShare() {
        try {
            if (usingScreen) return true;
            await getMedia(); // asegura cameraStream
            screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true, audio: false });
            const screenTrack = screenStream.getVideoTracks()[0];

            // cuando el usuario pare desde el UI del SO, restauramos cámara
            screenTrack.addEventListener("ended", () => stopScreenShare());

            // Cambia la pista saliente en todos los peers
            await replaceVideoTrackForAllPeers(screenTrack);

            // Cambia la vista local para que veas lo que compartes
            const lv = byId(els.localVideo);
            if (lv) { lv.srcObject = screenStream; try { await lv.play(); } catch { } }

            usingScreen = true;
        }
        catch (ex) {
            alert(ex)
        }
        return usingScreen;
    }

    // Detener compartir pantalla (volver a cámara)
    async function stopScreenShare() {
        if (!usingScreen) return false;
        if (screenStream) { screenStream.getTracks().forEach(t => t.stop()); screenStream = null; }
        await getMedia(); // asegura cameraStream
        const camTrack = cameraStream.getVideoTracks()[0];
        await replaceVideoTrackForAllPeers(camTrack);

        const lv = byId(els.localVideo);
        if (lv) { lv.srcObject = cameraStream; try { await lv.play(); } catch { } }

        usingScreen = false;
        return usingScreen;
    }

    // Silenciar / activar micrófono
    function toggleMute() {
        if (!localStream) return;
        localStream.getAudioTracks().forEach(t => t.enabled = !t.enabled);
        // devuelve estado útil para UI
        const muted = !localStream.getAudioTracks().every(t => t.enabled);
        return muted;
    }

    // Apagar / encender cámara (sin colgar la llamada)
    function toggleCamera() {
        if (!localStream) return;
        localStream.getVideoTracks().forEach(t => t.enabled = !t.enabled);
        const off = !localStream.getVideoTracks().every(t => t.enabled);
        return off;
    }

    // Exporta las nuevas funciones
    return {
        init, join, hangup, setEls: (m) => Object.assign(els, m),
        startScreenShare, stopScreenShare, toggleMute, toggleCamera
    };

})();

let wakeLock = null;

async function requestWakeLock() {
    try {
        if ('wakeLock' in navigator) {
            wakeLock = await navigator.wakeLock.request('screen');

            wakeLock.addEventListener('release', () => {
                console.log('Wake Lock was released');
            });

            console.log('Wake Lock is active');
        }
    } catch (err) {
        console.error(`${err.name}, ${err.message}`);
    }
}

async function releaseWakeLock() {
    if (wakeLock !== null) {
        await wakeLock.release();
        wakeLock = null;
    }
}
