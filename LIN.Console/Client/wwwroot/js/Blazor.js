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
