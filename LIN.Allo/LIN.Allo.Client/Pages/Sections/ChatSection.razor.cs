using LIN.Allo.Shared.Components.Elements.Popups;
using LIN.Allo.Shared.Services;

namespace LIN.Allo.Client.Pages.Sections;


public partial class ChatSection : IDisposable, IMessageChanger, IConversationViewer
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


    [Parameter]
    public Action EmmaClick { get; set; }



    /// <summary>
    /// Integrante del chat.
    /// </summary>
    [Parameter]
    public ConversationLocal Iam
    {
        get => _iam; set
        {
            _iam = value;
            InvokeAsync(() =>
            {
                ConversationsObserver.UnSuscribe((IMessageChanger)this);
                ConversationsObserver.UnSuscribe((IConversationViewer)this);

                if (value == null)
                    return;


                ConversationsObserver.Suscribe(value.Conversation.ID, (IMessageChanger)this);
                ConversationsObserver.Suscribe(value.Conversation.ID, (IConversationViewer)this);

                Message = string.Empty;
                StateHasChanged();
            });
        }
    }




    public ConversationLocal _iam;



    /// <summary>
    /// Acciona a ejecutar cuando se presione sobre el back button.
    /// </summary>
    [Parameter]
    public Action? OnBackPress { get; set; }




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
    private EmojiPanel? EmojiPanel { get; set; }




    void GetValue(dynamic e)
    {
        Message = e.Value;
    }




    /// <summary>
    /// Enviar un mensaje.
    /// </summary>
    private void SendMessage()
    {



        if (string.IsNullOrWhiteSpace(Message) || Iam == null)
            return;

        // Id único.
        var guid = Guid.NewGuid().ToString();

        // Generar evento.
        ConversationsObserver.PushMessage(Iam.Conversation.ID, new()
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
        RealTime.Hub?.SendMessage(Iam.Conversation.ID, Message, guid);

        // Reestablece el texto
        Message = "";

        // Actualiza la vista
        StateHasChanged();

        // Scroll al final
        _ = ScrollToBottom();

    }


    /// <summary>
    /// Enviar un mensaje.
    /// </summary>
    private void SendMessageKey(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs e)
    {

        if (e.Key == "Enter")
            SendMessage();


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


    protected override void OnInitialized()
    {
        base.OnInitialized();
        ConversationsObserver.Suscribe(Iam.Conversation.ID, (IMessageChanger)this);
        ConversationsObserver.Suscribe(Iam.Conversation.ID, (IConversationViewer)this);
    }


    /// <summary>
    /// Scroll hasta el final
    /// </summary>
    public async Task ScrollToBottom()
    {
        await JSRuntime.InvokeVoidAsync("scrollToBottom", $"CM-{Iam?.Conversation?.ID}");
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
        Drawer.SetDefault(Iam.Conversation.Name, Iam.Conversation.Type);

        await Drawer.LoadData(Iam.Conversation.ID);
        Drawer?.Show();
    }

    public void Dispose()
    {
        ConversationsObserver.UnSuscribe((IConversationViewer)this);
        ConversationsObserver.UnSuscribe((IMessageChanger)this);
    }

    public void Change()
    {
        InvokeAsync(StateHasChanged);
    }


    public void Change(string newName)
    {
        Iam.Conversation.Name = newName;
        StateHasChanged();
    }

}