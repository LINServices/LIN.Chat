scrollToBottom = function (elementId) {
    let element = document.getElementById(elementId);
    if (element) {
        let h = element.scrollHeight;
        element.scroll({
            top: h + 500,
            left: 0,
            behavior: "smooth"
        });
    }
};
function forceClick(id) {

    const control = document.getElementById(id);
    control.click();
}

function GoLaunch (url) {
    window.open(url, '_blank', 'noopener,noreferrer');
};


function getOperativeSystem() {
    const ua = navigator.userAgent.toLowerCase();

    if (/windows/.test(ua)) return "windows";
    if (/macintosh|mac os x/.test(ua)) return "macos";
    if (/android/.test(ua)) return "android";
    if (/iphone|ipad|ipod/.test(ua)) return "ios";
    if (/linux/.test(ua)) return "linux";

    return "otro";
}

function getBrowserName() {
    const ua = navigator.userAgent.toLowerCase();

    if (/edg\//.test(ua)) return "edge";
    if (/chrome/.test(ua) && !/edg\//.test(ua) && !/opr\//.test(ua)) return "chrome";
    if (/firefox/.test(ua)) return "firefox";
    if (/safari/.test(ua) && !/chrome/.test(ua)) return "safari";

    return "otro";
}