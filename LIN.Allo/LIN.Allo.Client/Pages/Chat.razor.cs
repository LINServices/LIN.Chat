using LIN.Allo.Shared.Components.Shared;
using LIN.Types.Cloud.Identity.Models.Identities;
using System.Net;

namespace LIN.Allo.Client.Pages;

public partial class Chat : IChatViewer
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
    private EmmaDrawer? EmmaDrawer { get; set; }


    /// <summary>
    /// Drawer de Miembros.
    /// </summary>
    private Members? MemberDrawer { get; set; }


    /// <summary>
    /// Al establecer los parámetros.
    /// </summary>
    protected override Task OnParametersSetAsync()
    {

        // Si el id es 0.
        if (Id <= 0)
        {
            SelectedConversation = null;
            ActualSection = 0;
            StateHasChanged();
            return Task.Run(() => { });
        }

        // Seleccionar una conversación.
        Select(Id, true);

        return base.OnParametersSetAsync();

    }



    /// <summary>
    /// Navegar al home.
    /// </summary>
    void Nav() => navigationManager.NavigateTo("/");



    /// <summary>
    /// Cuentas.
    /// </summary>
    public static List<AccountModel> Accounts { get; set; } = [];






    private NotificationBubble? notif;









    public static List<AccountModel> accounts = new();







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
        var result = await Access.Communication.Controllers.Members.SearchProfiles(Pattern, Access.Communication.Session.Instance.AccountToken);

        SearchResult = result.Models.Where(t => t.Profile.Id != Access.Communication.Session.Instance.Profile.Id).ToList();
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

            ConversationsObserver.PushMessage(e.Conversacion.Id, e);

        }
        catch
        {

        }



    }


    public async void OnReceiveCall(string s)
    {
        try
        {
            if (notif == null)
                return;

            if (CallSection.IsThisDeviceOnCall)
                return;

            notif.OnAccept = () =>
            {
                navigationManager.NavigateTo("/room/" + s);
            };
            await notif.Open();
        }
        catch
        {
        }
    }




    public void OnSuccess()
    {
        NewGroupModal?.Hide();
        ForceRetrieveData();
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

        // Obtener info del so.
        var so = await JSRuntime.InvokeAsync<int>("getOperativeSystem");
        var browser = await JSRuntime.InvokeAsync<int>("getBrowserName");

        Device.Platform = (Types.Enumerations.Platforms)so;
        Device.SurfaceFrom  = Types.Enumerations.SurfaceFrom.WebApp;
        Device.Browser = (Types.Enumerations.Browsers)browser;
        Device.Name = "Dispositivo web";

        // Iniciar el hub.
        if (!HubClient.Started)
        {
            await HubClient.SubscribeAsync(Access.Communication.Session.Instance.Profile);
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

        try
        {
            Nav();

            ConversationsObserver.Data.Clear();
            IsConversationsLoad = false;
            StateHasChanged();

            // Variables
            var profile = Access.Communication.Session.Instance.Profile;
            var token = Access.Communication.Session.Instance.Token ?? string.Empty;


            // Obtiene las conversaciones actuales
            var chats = await Access.Communication.Controllers.Conversations.ReadAll(token, Access.Communication.Session.Instance.AccountToken);

            try
            {
                //    chats.Alternatives = System.Text.Json.JsonSerializer.Deserialize<List<AccountModel>>(chats.Alternatives.ToString() ?? "");
                //    if (chats.AlternativeObject is List<AccountModel> lista)
                //    {
                //        accounts.AddRange(lista);
                //    }
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
            HubClient.OnCall(OnReceiveCall);
            HubClient.OnMessage(OnReceiveMessage);
            HubClient.OnCommand(OnCommand);

            // Suscribir los eventos del hub
            foreach (var conversation in chats.Models)
            {
                Suscribe(conversation.Conversation);
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




    public void Suscribe(ConversationModel conversation)
    {
        ConversationsObserver.Create(conversation);

        // Suscribir evento.
        HubClient.JoinGroupAsync(conversation.Id);

    }


    public async void OnCommand(string s)
    {
        try
        {
            if (CallSection.IsThisDeviceOnCall)
                return;

            navigationManager.NavigateTo("/room/" + s);
        }
        catch
        {
        }
    }







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
    public void Go(ConversationLocal chat)
    {
        Go(chat.Conversation.Id);
    }


    /// <summary>
    /// Seleccionar un chat
    /// </summary>
    /// <param name="chat"></param>
    public void Go(int chat)
    {

        if (chat == SelectedConversation?.Id)
        {
            navigationManager.NavigateTo(navigationManager.GetUriWithQueryParameter("Id", 0));
            return;
        }

        var uri = navigationManager.GetUriWithQueryParameter("Id", chat);
        navigationManager.NavigateTo(uri);
    }



    /// <summary>
    /// Seleccionar un chat
    /// </summary>
    /// <param name="chat"></param>
    public async void Select(int chat, bool force = false)
    {


        // Consulta al cache
        var cache = (from C in ConversationsObserver.Data
                     where C.Item2.Conversation.Id == chat
                     select C).FirstOrDefault().Item2;


        if (cache == null)
            return;


        IsSearching = false;
        StateHasChanged();


        if ((SelectedConversation?.Id == cache.Conversation.Id) && force)
        {

            return;
        }




        if (SelectedConversation?.Id == cache.Conversation.Id)
        {
            navigationManager.NavigateTo(navigationManager.GetUriWithQueryParameter("Id", 0));
            ////  cache.Control?.Unselect();
            //SelectedConversation = null;
            //ActualSection = 0;
            //StateHasChanged();
            return;
        }

        // Member
        SelectedConversation = null;
        StateHasChanged();
        //  await Task.Delay(50);

        SelectedConversation = cache.Conversation;

        // Si los chats (mensajes) no se han cargado.
        if (cache.Messages == null)
        {
            var oldMessages = await Access.Communication.Controllers.Messages.ReadAll(SelectedConversation.Id, 0, Access.Communication.Session.Instance.Token);

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


    public void RefreshUI()
    {
        StateHasChanged();
    }


}