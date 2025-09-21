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

    if (/iphone|ipad|ipod/.test(ua)) return 5;
    if (/windows/.test(ua)) return 1;
    if (/macintosh|mac os x/.test(ua)) return 3;
    if (/android/.test(ua)) return 2;
    if (/linux/.test(ua)) return 4;

    return 0;
}

function getBrowserName() {
    const ua = navigator.userAgent.toLowerCase();

    if (/edg\//.test(ua)) return 2;
    if (/chrome/.test(ua) && !/edg\//.test(ua) && !/opr\//.test(ua)) return 1;
    if (/firefox/.test(ua)) return 3;
    if (/safari/.test(ua) && !/chrome/.test(ua)) return 4;

    return 0;
}