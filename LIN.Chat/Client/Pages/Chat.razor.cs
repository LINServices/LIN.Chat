namespace LIN.Chat.Client.Pages;


public partial class Chat
{


    private static bool IsConversationsLoad { get; set; } = false;




    /// <summary>
    /// Lista de conversaciones
    /// </summary>
    private static readonly List<MemberChatModel> ConversaciónModels = new();



    /// <summary>
    /// Hub de conexión Realtime
    /// </summary>
    private static LIN.Access.Communication.Hubs.ChatHub? ActualHub { get; set; }



    /// <summary>
    /// Mi perfil
    /// </summary>
    private static MemberChatModel? Member { get; set; }



    /// <summary>
    /// Lista de Chats abiertos
    /// </summary>
    private static readonly Dictionary<int, (Access.Communication.Hubs.ChatHub, MemberChatModel, Status)> Chats = new();



    /// <summary>
    /// Pagina actual de Chat
    /// </summary>
    private static Shared.ChatSection? ChatPage { get; set; }



    /// <summary>
    /// Lista de componentes de acceso al chat
    /// </summary>
    private readonly static List<Shared.Control> ComponentRefs = new();



    /// <summary>
    /// Ref de un componente chat
    /// </summary>
    private static Shared.Control ComponentRef
    {
        set { ComponentRefs.Add(value); }
    }






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
    private async void ForceRetrieveData()
    {
        IsConversationsLoad = false;
        base.StateHasChanged();
        // Variables
        var profile = LIN.Access.Communication.Session.Instance.Informacion;
        string token = Access.Communication.Session.Instance.Token ?? string.Empty;

        // Obtiene las conversaciones actuales
        ReadAllResponse<MemberChatModel> chats = await Access.Communication.Controllers.Conversations.ReadAll(token);


        // Si hubo un error
        if (chats.Response != Responses.Success)
        {
            IsConversationsLoad = true;
            base.StateHasChanged();
            return;
        }


        // Lista
        Chats.Clear();
        ConversaciónModels.Clear();
        ConversaciónModels.AddRange(chats.Models);


        // Configuración del hub
        var hub = new LIN.Access.Communication.Hubs.ChatHub(profile);
        await hub.Suscribe();

        hub.OnReceiveMessage += OnReceiveMessage;

        // Suscribir los eventos del hub
        foreach (MemberChatModel conversation in ConversaciónModels)
        {
            // Configuración del modelo
            conversation.Profile = profile;
            conversation.Conversation.Mensajes ??= new();


            _ = hub.JoinGroup(conversation.Conversation.ID);

            // Agrega al cache
            Chats.Add(conversation.Conversation.ID, (hub, conversation, new() { IsLoad = false }));

        }

        // Actualiza la vista
        IsConversationsLoad = true;
        StateHasChanged();

    }

    private void OnReceiveMessage(object? sender, MessageModel e)
    {

        MemberChatModel? conversation = ConversaciónModels.Where(T => T.Conversation.ID == e.Conversacion.ID).FirstOrDefault();

        if (conversation == null)
            return;
        
        // Agrega el mensaje
        conversation.Conversation.Mensajes.Add(e);

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


        throw new NotImplementedException();
    }



    /// <summary>
    /// Obtiene la información del servidor
    /// </summary>
    private void RetrieveData()
    {

        // Si ya hay chats
        if (Chats.Count > 0)
            return;

        // Obtiene los datos
        ForceRetrieveData();

    }



    /// <summary>
    /// Seleccionar un chat
    /// </summary>
    /// <param name="chat"></param>
    private async void Select(MemberChatModel chat)
    {




        // Consulta al cache
        var cache = (from C in Chats
                     where C.Key == chat.Conversation.ID
                     select C.Value).FirstOrDefault();


        // Si son null
        switch (cache)
        {
            case (null, _, _) or (_, null, _) or (_, _, null):
                return;

            default:
                break;
        }
        if (Member == cache.Item2)
        {
            Member = null;
            StateHasChanged();
            return;
        }

        // Member
        Member = cache.Item2;
        ActualHub = cache.Item1;

        // Si los chats (mensajes) no se han cargado.
        if (!cache.Item3.IsLoad)
        {
            var oldMessages = await Access.Communication.Controllers.Messages.ReadAll(Member.Conversation.ID, 0, LIN.Access.Communication.Session.Instance.Token);

            // Establece los mensajes
            Member.Conversation.Mensajes.AddRange(oldMessages.Models);
            cache.Item3.IsLoad = true;
        }

        // Cambia la sección a (1)
        ActualSection = 1;
        base.StateHasChanged();

    }



    /// <summary>
    /// Clase de status
    /// </summary>
    private class Status
    {
        public bool IsLoad { get; set; }
    }


















}