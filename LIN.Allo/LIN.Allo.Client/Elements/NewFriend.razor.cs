﻿using LIN.Types.Cloud.Identity.Abstracts;

namespace LIN.Allo.Client.Elements;


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
    private string Img64 => Convert.ToBase64String(UserInformation?.Account.Profile ?? []);



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

        //Encuentra la conversación local.
        var localConversation = ConversationsObserver.Get(conversation.LastID);

        // Si existe local.
        if (localConversation != null)
        {
            // Seleccionar la conversación.
            Chat.Instance.IsSearching = false;
            Chat.Instance.Go(localConversation.Conversation.ID);
            return;
        }


        // Crear o encontrar la conversación en la API.
        var apiConversation = await Access.Communication.Controllers.Conversations.Read(conversation.LastID, session.Token, session.AccountToken);

        // Error.
        if (apiConversation.Response != Responses.Success)
        {
            Section = Sections.Error;
            StateHasChanged();
            return;
        }

        // Agregar información de las cuentas.
        if (apiConversation.AlternativeObject is List<AccountModel> Accounts)
            Chat.Accounts.AddRange(Accounts);

        // Modelo de conversación.


        Chat.Suscribe(apiConversation.Model.Conversation);

        Chat.Instance.IsSearching = false;
        Chat.Instance.Go(apiConversation.Model.Conversation.ID);

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