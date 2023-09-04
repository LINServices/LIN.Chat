

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







async function getLocationPermission (dotnetReference) {
        if ("permissions" in navigator) {
            await navigator.permissions.query({ name: "geolocation" }).then(function (result) {
                if (result.state === "granted" || result.state === "prompt") {
                    dotnetReference.invokeMethodAsync("LocationPermissionGranted");
                } else if (result.state === "denied") {
                    dotnetReference.invokeMethodAsync("LocationPermissionDenied");
                }
            });
        } else {
            dotnetReference.invokeMethodAsync("LocationPermissionNotSupported");
        }
    }


function getLocation(dotnetReference)
{
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(function (position) {
                console.log(position);
                dotnetReference.invokeMethodAsync("UpdateLocation", position.coords.latitude, position.coords.longitude);
            });
        } else {
            dotnetReference.invokeMethodAsync("LocationError", "Geolocation is not available in this browser.");
        }
    }