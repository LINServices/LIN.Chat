namespace LIN.Allo.Client.Pages.Sections;

public partial class CallSection
{
    /// <summary>
    /// Id de la llamada.
    /// </summary>
    [Parameter]
    public string RoomId { get; set; } = string.Empty;

    /// <summary>
    /// Cajon share.
    /// </summary>
    private DevicesDrawer? devicesDrawer;

    /// <summary>
    /// Estado del micrófono.
    /// </summary>
    private bool MicroState { get; set; } = true;

    /// <summary>
    /// Estado de la cámara.
    /// </summary>
    private bool CamState { get; set; } = true;

    /// <summary>
    /// Soporta compartir campaña.
    /// </summary>
    private bool SupportShareScreen = false;

    /// <summary>
    /// Obtener o establecer si el dispositivo esta en una llamada.
    /// </summary>
    public static bool IsThisDeviceOnCall { get; set; } = false;

    /// <summary>
    /// Obtener o establecer si el dispositivo esta compartiendo pantalla.
    /// </summary>
    public bool IsSharingScreen { get; set; } = false;

    /// <summary>
    /// Después de renderizar.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool first)
    {
        if (first)
        {
            SupportShareScreen = await JSRuntime.InvokeAsync<bool>("supportShareScreen");
            await JSRuntime.InvokeVoidAsync("webrtc.init", "https://api.linplatform.com/Communication/hub/calls", Access.Communication.Session.Instance.Account.Name);
            await JSRuntime.InvokeVoidAsync("webrtc.join", int.Parse(RoomId), LIN.Access.Communication.Session.Instance.Token); // asegura que RoomId está seteado
            IsThisDeviceOnCall = true;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Compartir / dejar de compartir pantalla.
    /// </summary>
    private async void ToggleShare()
    {
        if (IsSharingScreen)
        {
            await StopShare();
            return;
        }
        await ShareScreen();
    }

    /// <summary>
    /// Compartir pantalla.
    /// </summary>
    private async Task ShareScreen()
    {
        IsSharingScreen = await JSRuntime.InvokeAsync<bool>("webrtc.startScreenShare");
        StateHasChanged();
    }

    /// <summary>
    /// Dejar de compartir pantalla.
    /// </summary>
    private async Task StopShare()
    {
        IsSharingScreen = await JSRuntime.InvokeAsync<bool>("webrtc.stopScreenShare");
        StateHasChanged();
    }

    /// <summary>
    /// Activar / desactivar el microfono.
    /// </summary>
    private async Task ToggleMute()
    {
        var state = await JSRuntime.InvokeAsync<bool>("webrtc.toggleMute");
        MicroState = !state;
        StateHasChanged();
    }

    /// <summary>
    /// Activar / desactivar la camara.
    /// </summary>
    private async Task ToggleCamera()
    {
        var state = await JSRuntime.InvokeAsync<bool>("webrtc.toggleCamera");
        CamState = !state;
        StateHasChanged();
    }

    /// <summary>
    /// Colgar una llamada.
    /// </summary>
    private async void Hang()
    {
        await JSRuntime.InvokeVoidAsync("webrtc.hangup");
        NavigationContext.NavigateTo("/");
        IsThisDeviceOnCall = false;
    }

    /// <summary>
    /// Continuar en otro dispositivo.
    /// </summary>
    private void ContinueOn()
    {
        devicesDrawer?.Show();
    }

    /// <summary>
    /// Enviar evento para pasar la llamada.
    /// </summary>
    private void ContinueOn(DeviceOnAccountModel device)
    {
        Hang();
        HubClient.SendCommand(device.ConnectionId, RoomId);
    }
}