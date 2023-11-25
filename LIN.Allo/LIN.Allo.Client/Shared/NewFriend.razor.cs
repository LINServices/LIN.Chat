namespace LIN.Allo.Client.Shared;


public partial class NewFriend
{


    /// <summary>
    /// Información del usuario y el perfil.
    /// </summary>
    [Parameter]
    public SessionModel<ProfileModel>? UserInformation { get; set; } = null;



    /// <summary>
    /// Acción a realizar cuando se haga click.
    /// </summary>
    [Parameter]
    public Action<SessionModel<ProfileModel>> OnSelect { get; set; } = (e) =>
    {
    };



    /// <summary>
    /// Obtiene la imagen en Base64.
    /// </summary>
    private string Img64 => Convert.ToBase64String(UserInformation?.Account.Perfil ?? []);



    /// <summary>
    /// Si se cargando información.
    /// </summary>
    private Sections Section { get; set; } = Sections.Button;




    /// <summary>
    /// Encuentra una conversación.
    /// </summary>
    private async void Find()
    {

        // Sesión.
        var session = Access.Communication.Session.Instance;

        // Validar los parámetros disponibles.
        if (UserInformation == null || Chat.Instance == null)
            return;

        // Cambia los estados.
        Section = Sections.Loading;
        StateHasChanged();

        // Obtiene la información de la conversación.
        var conversation = await Access.Communication.Controllers.Conversations.Find(UserInformation.Profile.ID, Access.Communication.Session.Instance.Token);

        // Error.
        if (conversation.Response != Responses.Success)
        {
            Section = Sections.Error;
            StateHasChanged();
            return;
        }

        // Encuentra la conversación local.
        var localConversation = Chat.Conversations.FirstOrDefault(c => c.Id == conversation.LastID);

        // Si existe local.
        if (localConversation != null)
        {
            // Seleccionar la conversación.
            Chat.Instance.IsSearching = false;
            Chat.Instance.Select(localConversation.Id);
            return;
        }


        // Crear o encontrar la conversación en la API.
        var apiConversation = await Access.Communication.Controllers.Conversations.Read(conversation.LastID, session.Token, session.AccountToken);

        // Agregar información de las cuentas.
        if (apiConversation.AlternativeObject is List<AccountModel> accounts)
            Chat.accounts.AddRange(accounts);

        // Modelo de conversación.
        Chat.Conversations.Add(new()
        {
            Control = null,
            IsLoad = false,
            Id = conversation.LastID,
            Chat = apiConversation.Model
        });

        // Suscribir el evento.
        _ = ChatSection.Hub!.JoinGroup(conversation.LastID);

        // Cambiar los estados.
        Chat.Instance.IsSearching = false;
        Chat.Instance?.StateChange();
        Chat.Instance?.Select(conversation.LastID);

    }



    /// <summary>
    /// Secciones.
    /// </summary>
    enum Sections
    {
        Button,
        Loading,
        Error
    }



}