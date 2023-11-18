namespace LIN.Allo.Client.Pages;


public partial class Chat
{

    /// <summary>
    /// Sección actual del chat
    /// </summary>
    private static ChatSection? ChatPage { get; set; }













    private static bool IsConversationsLoad { get; set; } = false;




    /// <summary>
    /// Lista de conversaciones
    /// </summary>
    private readonly static List<MemberChatModel> ConversaciónModels = new();


    /// <summary>
    /// Mi perfil
    /// </summary>
    private static MemberChatModel? Member { get; set; }



    /// <summary>
    /// Lista de Chats abiertos
    /// </summary>
    private readonly static Dictionary<int, (Access.Communication.Hubs.ChatHub, MemberChatModel, Status)> Chats = new();






    /// <summary>
    /// Lista de componentes de acceso al chat
    /// </summary>
    public readonly static List<Shared.Control> ComponentRefs = new();



    /// <summary>
    /// Ref de un componente chat
    /// </summary>
    private static Shared.Control ComponentRef
    {
        set
        {

            ComponentRefs.RemoveAll(x => x.Member.Conversation.ID == value.Member.Conversation.ID);
            ComponentRefs.Add(value);
        }
    }






    /// <summary>
    /// Cuando la pagina se inicia
    /// </summary>
    protected override async void OnInitialized()
    {

        // Valida el login
        if (!Access.Communication.Session.IsOpen)
        {
            navigationManager.NavigateTo("/login");
            return;
        }

        // Crear el hub
        ChatSection.Hub = new(Access.Communication.Session.Instance.Profile);
        await ChatSection.Hub.Suscribe();

        // Obtiene la data
        RetrieveData();

        base.OnInitialized();
    }



    /// <summary>
    /// Obtiene la información del servidor
    /// </summary>
    private async void ForceRetrieveData()
    {


        ChatSection.Hub!.OnReceiveMessage ??= new();
        ChatSection.Hub!.OnReceiveMessage?.Clear();
        IsConversationsLoad = false;
        StateHasChanged();

        // Variables
        var profile = Access.Communication.Session.Instance.Profile;
        var token = Access.Communication.Session.Instance.Token ?? string.Empty;



        // Obtiene las conversaciones actuales
        var chats = await Access.Communication.Controllers.Conversations.ReadAll(token);


        // Si hubo un error
        if (chats.Response != Responses.Success)
        {
            IsConversationsLoad = true;
            StateHasChanged();
            return;
        }


        // Lista
        Chats.Clear();
        ConversaciónModels.Clear();
        ConversaciónModels.AddRange(chats.Models);

        ChatSection.Hub!.OnReceiveMessage?.Add(OnReceiveMessage);


        // Suscribir los eventos del hub
        foreach (var conversation in ConversaciónModels)
        {
            // Configuración del modelo
            conversation.Profile = profile;
            conversation.Conversation.Mensajes ??= new();


            _ = ChatSection.Hub!.JoinGroup(conversation.Conversation.ID);

            // Agrega al cache
            Chats.Add(conversation.Conversation.ID, (ChatSection.Hub!, conversation, new()
            {
                IsLoad = false
            }));

        }

        // Actualiza la vista
        IsConversationsLoad = true;
        StateHasChanged();

    }

    public static List<Shared.Message> MessageTasker = new();


    public static void OnReceiveMessage(MessageModel e)
    {

        var conversation = ConversaciónModels.Where(T => T.Conversation.ID == e.Conversacion.ID).FirstOrDefault();

        if (conversation == null)
            return;

        // Agrega el mensaje
        var exist = conversation.Conversation.Mensajes.FirstOrDefault(T => T.Guid == e.Guid);

        if (exist == null)
        {
            conversation.Conversation.Mensajes.Add(e);
        }
        else
        {
            var inTask = MessageTasker.FirstOrDefault(T => T.MessageModel.Guid == e.Guid);
            inTask?.UnLocal();
        }



        if (e.Contenido.StartsWith("#"))
        {
            var app = new SILF.Script.App(e.Contenido.Remove(0, 1));
            //  app.AddDefaultFunctions(Online.Scripts.Actions);
            app.Run();
        }

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


        var cm = ComponentRefs.Where(T => T.Member.Conversation.ID == chat.Conversation.ID).FirstOrDefault();

        foreach (var c in ComponentRefs)
            c.Unselect();

        if (Member?.Conversation.ID == cache.Item2.Conversation.ID)
        {
            cm?.Unselect();
            Member = null;
            ActualSection = 0;
            StateHasChanged();
            return;
        }

        // Member
        Member = cache.Item2;
        cm?.Select();

        // Si los chats (mensajes) no se han cargado.
        if (!cache.Item3.IsLoad)
        {
            var oldMessages = await Access.Communication.Controllers.Messages.ReadAll(Member.Conversation.ID, 0, Access.Communication.Session.Instance.Token);

            // Establece los mensajes
            Member.Conversation.Mensajes.AddRange(oldMessages.Models);
            cache.Item3.IsLoad = true;
        }

        // Cambia la sección a (1)
        ActualSection = 1;
        StateHasChanged();

    }



    /// <summary>
    /// Clase de status
    /// </summary>
    private class Status
    {
        public bool IsLoad { get; set; }
    }


















}