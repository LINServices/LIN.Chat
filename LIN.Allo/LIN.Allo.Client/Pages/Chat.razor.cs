namespace LIN.Allo.Client.Pages;


public partial class Chat
{

    //======== Modales =========//

    /// <summary>
    /// Drawer de nuevo grupo.
    /// </summary>
    private NewGroup? NewGroupModal { get; set; }


    /// <summary>
    /// Drawer de Emma.
    /// </summary>
    private Emma? EmmaDrawer { get; set; }


    /// <summary>
    /// Drawer de Miembros.
    /// </summary>
    private Members? MemberDrawer { get; set; }


    /// <summary>
    /// Sección actual del chat
    /// </summary>
    private static ChatSection? ChatPage { get; set; }



    //======== Propiedades =========//


    /// <summary>
    /// Patron de búsqueda.
    /// </summary>
    private string Pattern { get; set; } = string.Empty;


    /// <summary>
    /// Sección grafica actual.
    /// </summary>
    private int ActualSection { get; set; } = 0;


    /// <summary>
    /// Las conversaciones están cargadas.
    /// </summary>
    private static bool IsConversationsLoad { get; set; }


    /// <summary>
    /// Imagen de perfil en base64
    /// </summary>
    private static string Img64 => Convert.ToBase64String(Access.Communication.Session.Instance.Account.Perfil);


    /// <summary>
    /// Lista de conversaciones.
    /// </summary>
    private static List<InteractiveConversation> Conversations { get; set; } = new();


    /// <summary>
    /// Conversación seleccionada.
    /// </summary>
    private MemberChatModel? SelectedConversation { get; set; }


    /// <summary>
    /// Esta buscando.
    /// </summary>
    private bool IsSearching { get; set; }


    /// <summary>
    /// Ref de un componente chat
    /// </summary>
    private static Control ComponentRef
    {
        set
        {
            // Obtiene el elemento.
            var element = Conversations.Where(t => t.Id == value.Member.Conversation.ID).FirstOrDefault();

            // Si no existe.
            if (element == null)
                return;

            // Establece el control.
            element.Control = value;
        }
    }




    /// <summary>
    /// Abrir el cajon de nuevo grupo.
    /// </summary>
    private void OpenNewGroup()
    {
        NewGroupModal?.Show();
    }



    /// <summary>
    /// Abrir el cajon de nuevo grupo.
    /// </summary>
    private void OpenEmma()
    {
        EmmaDrawer?.Show();
    }



    /// <summary>
    /// Lista de resultados de búsqueda.
    /// </summary>
    private List<Types.Auth.Abstracts.SessionModel<ProfileModel>>? SearchResult { get; set; }



    /// <summary>
    /// Contador.
    /// </summary>
    int counter = 0;


    /// <summary>
    /// Buscar.
    /// </summary>
    private async void Search(dynamic e)
    {

        counter++;

        var c = await Task.Run(async () =>
        {
            int save = counter;
            await Task.Delay(300);
            return (save == counter);
        });

        if (!c)
            return;

        if (e is ChangeEventArgs a)
        {
            Pattern = a.Value?.ToString() ?? "";
        }

        if (Pattern.Trim() == "")
        {
            SearchResult = null;
            IsSearching = false;
            StateHasChanged();
            return;
        }

        SearchResult = null;
        IsSearching = true;
        StateHasChanged();
        var result = await Access.Communication.Controllers.Conversations.SearchProfiles(Pattern, Access.Communication.Session.Instance.AccountToken);

        SearchResult = result.Models.Where(t=>t.Profile.ID != LIN.Access.Communication.Session.Instance.Profile.ID).ToList();
        counter = 0;
        StateHasChanged();

    }


    /// <summary>
    /// Evento al recibir un mensaje.
    /// </summary>
    /// <param name="e">Modelo del mensaje.</param>
    public static void OnReceiveMessage(MessageModel e)
    {

        // Obtiene la conversación.
        var element = Conversations.Where(c => c.Id == e.Conversacion.ID).FirstOrDefault();

        // No existe el elemento.
        if (element == null)
            return;

        // Obtiene el mensaje.
        var message = element.Chat.Conversation.Mensajes.FirstOrDefault(m => m.Guid == e.Guid);

        // El mensaje no existía.
        if (message == null)
            element.Chat.Conversation.Mensajes.Add(e);

        // El mensaje ya se había enviado.
        else
        {
            // Confirmar el mensaje UI.
            var inTask = MessageTasker.FirstOrDefault(T => T.MessageModel.Guid == e.Guid);
            inTask?.UnLocal();
        }

        // Si el mensaje es un método.
        if (e.Contenido.StartsWith("#"))
        {
            var app = new SILF.Script.App(e.Contenido.Remove(0, 1));
            //  app.AddDefaultFunctions(Online.Scripts.Actions);
            app.Run();
        }

        // Si la pagina actual es la misma a la cual llego el mensaje
        if (ChatPage?.Iam.Conversation.ID == element.Id)
        {
            ChatPage?.Render();
            ChatPage?.ScrollToBottom();
            return;
        }

        // Mostrar badge de nuevo mensaje.
        if (element.Control != null)
        {
            element.Control.IsNew = true;
            element.Control.Render();
        }

    }












    private async Task E()
    {

        await JSRuntime.InvokeAsync<object>("E");

        StateHasChanged();

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
        ChatSection.Hub!.OnReceiveMessage.Clear();
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
        Conversations.Clear();

        ChatSection.Hub!.OnReceiveMessage?.Add(OnReceiveMessage);


        // Suscribir los eventos del hub
        foreach (var conversation in chats.Models)
        {
            // Configuración del modelo
            conversation.Profile = profile;
            conversation.Conversation.Mensajes ??= new();

            _ = ChatSection.Hub!.JoinGroup(conversation.Conversation.ID);

            // Agregar los estados.
            Conversations.Add(new()
            {
                Id = conversation.Conversation.ID,
                Chat = conversation,
                Control = null,
                IsLoad = false
            });
        }

        // Actualiza la vista
        IsConversationsLoad = true;
        StateHasChanged();

    }

    public static List<Shared.Message> MessageTasker { get; set; } = new();








    /// <summary>
    /// Obtiene la información del servidor
    /// </summary>
    private void RetrieveData()
    {

        // Si ya hay chats
        if (IsConversationsLoad)
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
        var cache = (from C in Conversations
                     where C.Id == chat.Conversation.ID
                     select C).FirstOrDefault();


        if (cache == null)
            return;

        foreach (var c in Conversations)
            c.Control?.Unselect();




        if (SelectedConversation?.Conversation.ID == cache.Id)
        {
            cache.Control?.Unselect();
            SelectedConversation = null;
            ActualSection = 0;
            StateHasChanged();
            return;
        }

        // Member
        SelectedConversation = cache.Chat;
        cache.Control?.Select();

        // Si los chats (mensajes) no se han cargado.
        if (!cache.IsLoad)
        {
            var oldMessages = await Access.Communication.Controllers.Messages.ReadAll(SelectedConversation.Conversation.ID, 0, Access.Communication.Session.Instance.Token);

            // Establece los mensajes
            SelectedConversation.Conversation.Mensajes.AddRange(oldMessages.Models);
            cache.IsLoad = true;
        }

        // Cambia la sección a (1)
        ActualSection = 1;
        StateHasChanged();

    }





    class InteractiveConversation
    {
        public int Id { get; set; }
        public MemberChatModel Chat { get; set; }
        public bool IsLoad { get; set; }
        public Control? Control { get; set; }
    }







}