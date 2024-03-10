
using LIN.Allo.Client.Elements;

namespace LIN.Allo.Client.Pages;


public partial class Chat
{

    [Parameter]
    [SupplyParameterFromQuery]
    public int Id { get; set; }


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
    /// Sección actual del chat.
    /// </summary>
    private static ChatSection? ChatPage { get; set; }






    protected override Task OnParametersSetAsync()
    {


        if (Id <= 0)
        {
            SelectedConversation = null;
            ActualSection = 0;
            StateHasChanged();
            return Task.Run(() => { });
        }



        Select(Id);

        return base.OnParametersSetAsync();


    }


    void Nav()
    {
        navigationManager.NavigateTo("/home");
    }













    public static Chat? Instance { get; set; }



    public static List<AccountModel> accounts = new List<AccountModel>();







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
    private static string Img64 => Convert.ToBase64String(Access.Communication.Session.Instance.Account.Profile);


    /// <summary>
    /// Conversación seleccionada.
    /// </summary>
    private ConversationModel? SelectedConversation { get; set; }


    /// <summary>
    /// Esta buscando.
    /// </summary>
    public bool IsSearching { get; set; }






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
    private List<SessionModel<ProfileModel>>? SearchResult { get; set; }



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
            return save == counter;
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

        SearchResult = result.Models.Where(t => t.Profile.ID != Access.Communication.Session.Instance.Profile.ID).ToList();
        counter = 0;
        StateHasChanged();

    }



    /// <summary>
    /// Evento al recibir un mensaje.
    /// </summary>
    /// <param name="e">Modelo del mensaje.</param>
    public static void OnReceiveMessage(MessageModel e)
    {

        try
        {

            ConversationsObserver.PushMessage(e.Conversacion.ID, e);

        }
        catch
        {

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
            //navigationManager.NavigateTo("/login");
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

        try
        {
            ChatSection.Hub!.OnReceiveMessage ??= new();
            ChatSection.Hub!.OnReceiveMessage.Clear();
            IsConversationsLoad = false;
            StateHasChanged();

            // Variables
            var profile = Access.Communication.Session.Instance.Profile;
            var token = Access.Communication.Session.Instance.Token ?? string.Empty;


            // Obtiene las conversaciones actuales
            var chats = await Access.Communication.Controllers.Conversations.ReadAll(token, Access.Communication.Session.Instance.AccountToken);

            try
            {
                chats.AlternativeObject = System.Text.Json.JsonSerializer.Deserialize<List<AccountModel>>(chats.AlternativeObject.ToString() ?? "");
                if (chats.AlternativeObject is List<AccountModel> lista)
                {
                    accounts.AddRange(lista);
                }

            }
            catch
            {

            }
            // 


            // Si hubo un error
            if (chats.Response != Responses.Success)
            {
                IsConversationsLoad = true;
                StateHasChanged();
                return;
            }


            // Lista.
            ChatSection.Hub!.OnReceiveMessage?.Add(OnReceiveMessage);


            // Suscribir los eventos del hub
            foreach (var conversation in chats.Models)
            {


                ConversationsObserver.Create(conversation.Conversation);

                // Suscribir evento.
                _ = ChatSection.Hub!.JoinGroup(conversation.Conversation.ID);

            }

            // Actualiza la vista
            IsConversationsLoad = true;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            var s = ex;
        }



    }

    public static List<Message> MessageTasker { get; set; } = new();








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
    private void Go(ConversationLocal chat)
    {
        var uri = navigationManager.GetUriWithQueryParameter("Id", chat.Conversation.ID);
        navigationManager.NavigateTo(uri);
    }



    /// <summary>
    /// Seleccionar un chat
    /// </summary>
    /// <param name="chat"></param>
    private void Select(ConversationModel chat)
    {
        Select(chat.ID);
    }



    /// <summary>
    /// Seleccionar un chat
    /// </summary>
    /// <param name="chat"></param>
    public async void Select(int chat)
    {


        // Consulta al cache
        var cache = (from C in ConversationsObserver.Data.Values
                     where C.Conversation.ID == chat
                     select C).FirstOrDefault();


        if (cache == null)
            return;

        //foreach (var c in Conversations)
        //    c.Control?.Unselect();




        if (SelectedConversation?.ID == cache.Conversation.ID)
        {
            //  cache.Control?.Unselect();
            SelectedConversation = null;
            ActualSection = 0;
            StateHasChanged();
            return;
        }

        // Member
        SelectedConversation = null;
        StateHasChanged();
        //  await Task.Delay(50);

        SelectedConversation = cache.Conversation;
        // cache.Control?.Select();

        // Si los chats (mensajes) no se han cargado.
        if (cache.Messages == null)
        {
            var oldMessages = await Access.Communication.Controllers.Messages.ReadAll(SelectedConversation.ID, 0, Access.Communication.Session.Instance.Token);

            // Establece los mensajes
            cache.Messages = oldMessages.Models;
            // cache.IsLoad = true;
        }

        // Cambia la sección a (1)
        ActualSection = 1;
        StateHasChanged();
    }




    public void StateChange()
    {
        StateHasChanged();
    }

    void Close()
    {
        LIN.Access.Communication.Session.CloseSession();
        navigationManager.NavigateTo("/");
    }

}