namespace LIN.Chat.Client.Shared;

public partial class ChatSection
{


    private double latitude;
    private double longitude;
    private bool locationPermissionGranted = false;



    private async Task RequestLocationPermission()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("getLocationPermission", DotNetObjectReference.Create(this));
        }
        catch (Exception ex)
        {
            // Manejar errores aquí
        }
    }

    [JSInvokable]
    public void LocationPermissionGranted()
    {
        locationPermissionGranted = true;
        StateHasChanged();
    }

    [JSInvokable]
    public void LocationPermissionDenied()
    {
        locationPermissionGranted = false;
        StateHasChanged();
    }

    [JSInvokable]
    public void LocationPermissionNotSupported()
    {
        // Manejar el caso en el que el navegador no soporta el permiso de geolocalización
    }

    private async Task GetLocation()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("getLocation", DotNetObjectReference.Create(this));
        }
        catch (Exception ex)
        {
            // Manejar errores aquí
        }
    }


    [JSInvokable("UpdateLocation")]
    public void A(double a, double b)
    {
        latitude = a;
        longitude = b;
        StateHasChanged();
    }
}
