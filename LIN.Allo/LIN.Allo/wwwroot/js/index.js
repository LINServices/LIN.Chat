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




