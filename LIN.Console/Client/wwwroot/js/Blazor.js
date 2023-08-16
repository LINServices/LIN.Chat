// _Imports.razor (o cualquier archivo JavaScript)
window.BlazorDownloadFile = (fileName, contentType, downloadUrl) => {
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = fileName;
    link.target = '_blank';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};



function maper(v1, v2) {
    // Obtenemos el iframe


    var iframe = document.getElementById('mapper');

    iframe.contentWindow.postMessage(v1+"|"+v2, '*');

}
