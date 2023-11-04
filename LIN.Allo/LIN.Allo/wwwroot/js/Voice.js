var recognition = "";

// Verificar la compatibilidad del navegador
if ('SpeechRecognition' in window || 'webkitSpeechRecognition' in window) {
    recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();

    // Establecer el idioma a espa�ol (Espa�a)
    recognition.lang = 'es-ES'; // Puedes cambiar esto seg�n tu preferencia


} else {
    console.log("La API de reconocimiento de voz no es compatible con este navegador.");
}


function StartVoice(dotnetHelper) {

    recognition.onresult = (event) => {
        const transcript = event.results[0][0].transcript;

        dotnetHelper.invokeMethodAsync("OnEmma", transcript);
        console.log(transcript);
    };

    recognition.start();
}


function Speech(textToSpeak) {

// Crear un objeto de s�ntesis de voz
    const synth = window.speechSynthesis;
    const utterance = new SpeechSynthesisUtterance(textToSpeak);

// Establecer el idioma
    utterance.lang = 'es-ES'; // Puedes cambiar esto seg�n tu preferencia

// Reproducir el discurso
    synth.speak(utterance);
}
