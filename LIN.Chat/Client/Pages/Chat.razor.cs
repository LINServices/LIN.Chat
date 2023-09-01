namespace LIN.Chat.Client.Pages;


public partial class Chat
{


    /// <summary>
    /// Lista de conversaciones
    /// </summary>
    private readonly List<MemberChatModel> ConversaciónModels = new();



    /// <summary>
    /// Hub de conexión Realtime
    /// </summary>
    private LIN.Access.Communication.Hubs.ChatHub? ActualHub { get; set; }



    /// <summary>
    /// Mi perfil
    /// </summary>
    private MemberChatModel? Member { get; set; }



    /// <summary>
    /// Lista de Chats abiertos
    /// </summary>
    private readonly Dictionary<int, (Access.Communication.Hubs.ChatHub, MemberChatModel, A)> Chats = new();



    /// <summary>
    /// Pagina actual de Chat
    /// </summary>
    private Shared.ChatSection? ChatPage { get; set; }



    /// <summary>
    /// Cuando la pagina se inicia
    /// </summary>
    protected override void OnInitialized()
    {

        // Valida el login
        if (!LIN.Access.Communication.Session.IsOpen)
        {
            nav.NavigateTo("/login");
            return;
        }

        // Obtiene la data
        RetrieveData();

        base.OnInitialized();
    }



    /// <summary>
    /// Obtiene la información del servidor
    /// </summary>
    private async void RetrieveData()
    {

        // Variables
        var profile = LIN.Access.Communication.Session.Instance.Informacion;
        string token = Access.Communication.Session.Instance.Token ?? string.Empty;

        // Obtiene las conversaciones actuales
        ReadAllResponse<MemberChatModel> chats = await Access.Communication.Controllers.Conversations.ReadAll(token);

        // Si hubo un error
        if (chats.Response != Responses.Success)
            return;

        // Lista
        Chats.Clear();
        ConversaciónModels.Clear();
        ConversaciónModels.AddRange(chats.Models);


        // Configuración del hub
        var hub = new LIN.Access.Communication.Hubs.ChatHub();
        await hub.Suscribe();
        await hub.ConnectMe(profile);


        // Suscribir los eventos del hub
        foreach (MemberChatModel conversation in ConversaciónModels)
        {
            // Configuración del modelo
            conversation.Profile = profile;
            conversation.Conversation.Mensajes ??= new();

            // Evento de mensajes
            hub.JoinGroup(conversation.Conversation.ID.ToString(), (message) =>
            {
                // Agrega el mensaje
                conversation.Conversation.Mensajes.Add(message);

                // Si la pagina actual es la misma a la cual llego el mensaje
                if (ChatPage?.Iam.Conversation.ID == conversation.Conversation.ID)
                {
                    ChatPage?.Render();
                    ChatPage?.ScrollToBottom();
                    return;
                }

                // Obtiene el componente
                var component = ComponentRefs.Where(T => T.Member.Conversation.ID == conversation.Conversation.ID).FirstOrDefault();

                if (component != null)
                {
                    component.IsNew = true;
                    component.Render();
                }

            });

            // Agrega al cache
            Chats.Add(conversation.Conversation.ID, (hub, conversation, new() { IsLoad = false }));

        }

        // Actualiza la vista
        StateHasChanged();

    }


}