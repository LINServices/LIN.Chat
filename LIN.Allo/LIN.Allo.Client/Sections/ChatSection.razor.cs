namespace LIN.Allo.Client.Sections;


public partial class ChatSection
{

    /// <summary>
    /// Solo la fecha de hoy.
    /// </summary>

    public static DateTime Today = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);



    /// <summary>
    /// Drawer de integrantes
    /// </summary>
    [Parameter]
    public Members? Drawer { get; set; }



    /// <summary>
    /// Integrante del chat.
    /// </summary>
    [Parameter]
    public MemberChatModel Iam { get; set; } = new();



    /// <summary>
    /// Acciona a ejecutar cuando se presione sobre el back button.
    /// </summary>
    [Parameter]
    public Action? OnBackPress { get; set; }



    /// <summary>
    /// Hub de conexión.
    /// </summary>
    public static Access.Communication.Hubs.ChatHub? Hub { get; set; }



    /// <summary>
    /// Mensaje a enviar.
    /// </summary>
    private string Message { get; set; } = string.Empty;



    /// <summary>
    /// Fecha.
    /// </summary>
    private DateTime? oldTime = null;



    /// <summary>
    /// Panel de emojis.
    /// </summary>
    private Pops.EmojiPanel? EmojiPanel { get; set; }





    /// <summary>
    /// Enviar un mensaje.
    /// </summary>
    private void SendMessage()
    {

        if (string.IsNullOrWhiteSpace(Message))
            return;

        // Id único.
        var guid = Guid.NewGuid().ToString();

        // Generar evento.
        Chat.OnReceiveMessage(new()
        {
            Contenido = Message,
            Conversacion = new()
            {
                ID = Iam.Conversation.ID
            },
            Remitente = Access.Communication.Session.Instance.Profile,
            Time = DateTime.Now,
            Guid = guid,
            IsLocal = true
        });

        // Envía el mensaje al hub
        Hub?.SendMessage(Iam.Conversation.ID, Message, guid);

        // Reestablece el texto
        Message = "";

        // Actualiza la vista
        StateHasChanged();

        // Scroll al final
        _ = ScrollToBottom();

    }



    /// <summary>
    /// Ciclo de vida: Después de renderizar
    /// </summary>
    /// <param name="firstRender">Es el primer render</param>
    protected override void OnAfterRender(bool firstRender)
    {
        _ = ScrollToBottom();
        base.OnAfterRender(firstRender);
    }



    /// <summary>
    /// El estado ha cambiado
    /// </summary>
    public void Render()
    {
        StateHasChanged();
    }



    /// <summary>
    /// Scroll hasta el final
    /// </summary>
    public async Task ScrollToBottom()
    {
        await JSRuntime.InvokeVoidAsync("scrollToBottom", $"CM-{Iam.Conversation.ID}");
    }



    /// <summary>
    /// Abre el cajon de integrantes.
    /// </summary>
    private async void OpenDrawer()
    {

        // Si el drawer no esta iniciado.
        if (Drawer == null)
            return;

        // Establecer las propiedades.
        Drawer.Name = Iam.Conversation.Name;
        await Drawer.LoadData(Iam.Conversation.ID);
        Drawer?.Show();
    }


}