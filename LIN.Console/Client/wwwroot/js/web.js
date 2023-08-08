var agenteUsuario = navigator.userAgent;
var nombreNavegador, versionNavegador;


console.log("User: ", agenteUsuario);

if (agenteUsuario.indexOf("Firefox") !== -1) {
    nombreNavegador = "Mozilla Firefox";
    versionNavegador = agenteUsuario.match(/Firefox\/([\d.]+)/)[1];
} else if (agenteUsuario.indexOf("Chrome") !== -1) {
    nombreNavegador = "Google Chrome";
    versionNavegador = agenteUsuario.match(/Chrome\/([\d.]+)/)[1];
} else if (agenteUsuario.indexOf("Safari") !== -1) {
    nombreNavegador = "Safari";
    versionNavegador = agenteUsuario.match(/Version\/([\d.]+)/)[1];
} else if (agenteUsuario.indexOf("MSIE") !== -1 || agenteUsuario.indexOf("Trident/") !== -1) {
    nombreNavegador = "Internet Explorer";
    versionNavegador = agenteUsuario.match(/(?:MSIE |rv:)(\d+(\.\d+)?)/)[1];
} else {
    nombreNavegador = "Navegador desconocido";
    versionNavegador = "N/A";
}

console.log("Nombre del navegador:", nombreNavegador);
console.log("Versión del navegador:", versionNavegador);


function getNavegador() {
    return nombreNavegador;
}

function getVersion() {
    return versionNavegador;
}