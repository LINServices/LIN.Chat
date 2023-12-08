namespace LIN.Allo.Client.Elements.Drawers;

public partial class NewGroup
{
   
    /// <summary>
    /// Correcto.
    /// </summary>
    [Parameter]
    public Action? OnSuccess { get; set; }



    /// <summary>
    /// Id único del elemento.
    /// </summary>
    private string UniqueId { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Estado actual.
    /// </summary>
    private State ActualState = State.Modeling;



    /// <summary>
    /// Lista de estados.
    /// </summary>
    private enum State
    {
        Modeling,
        Creating,
        Success,
        Failure
    }



    /// <summary>
    /// Estado (Buscando).
    /// </summary>
    private bool IsSearching { get; set; } = false;



    /// <summary>
    /// Estado (Buscado).
    /// </summary>
    private bool AreSearch { get; set; } = false;



    /// <summary>
    /// Items seleccionados.
    /// </summary>
    private List<Types.Identity.Abstracts.SessionModel<ProfileModel>> SelectedItems { get; set; } = new();



    /// <summary>
    /// Lista de modelos de integrantes.
    /// </summary>
    private List<Types.Identity.Abstracts.SessionModel<ProfileModel>> MemberModels { get; set; } = new();



    /// <summary>
    /// Lista de controles de integrantes.
    /// </summary>
    private List<Profile> MemberControls { get; set; } = new();



    /// <summary>
    /// Control de integrante actual.
    /// </summary>
    private Profile MemberControl
    {
        set => MemberControls.Add(value);
    }



    /// <summary>
    /// Patron de búsqueda.
    /// </summary>
    private string pattern = "";



    /// <summary>
    /// Patron de búsqueda.
    /// </summary>
    private string Pattern
    {
        get => pattern;
        set
        {
            pattern = value;
            AreSearch = false;
            StateHasChanged();
        }
    }



    /// <summary>
    /// Nombre de la conversación.
    /// </summary>
    private string Name { get; set; } = string.Empty;



    /// <summary>
    /// Abrir el cajon.
    /// </summary>
    public async void Show()
    {
        await js.InvokeAsync<object>("ShowDrawer", $"drawerIG-{UniqueId}", $"close-drawerIG-{UniqueId}");
        StateHasChanged();
    }



    /// <summary>
    /// Al seleccionar un elemento.
    /// </summary>
    /// <param name="model">Modelo seleccionado.</param>
    private void OnSelect(Types.Identity.Abstracts.SessionModel<ProfileModel> model)
    {
        // Si existe.
        var have = SelectedItems.Where(T => T.Account.ID == model.Account.ID).Any();

        if (have)
            return;

        SelectedItems.Add(model);
        StateHasChanged();
    }



    /// <summary>
    /// Al eliminar.
    /// </summary>
    /// <param name="model">Modelo.</param>
    private void OnRemove(Types.Identity.Abstracts.SessionModel<ProfileModel> model)
    {
        SelectedItems = SelectedItems.Where(T => T.Account.ID != model.Account.ID).ToList();
        StateHasChanged();
    }



    /// <summary>
    /// Evento click
    /// </summary>
    private void Click()
    {
        AreSearch = false;
        IsSearching = false;
        MemberModels?.Clear();
        SelectedItems?.Clear();
        Name = "";
        Pattern = "";
        ActualState = State.Modeling;
        StateHasChanged();
    }



    /// <summary>
    /// Buscar elementos.
    /// </summary>
    private async void Search()
    {

        if (pattern.Trim() == "")
        {
            AreSearch = false;
            IsSearching = false;
            StateHasChanged();
            return;
        }

        AreSearch = true;
        IsSearching = true;
        StateHasChanged();
        var result = await Access.Communication.Controllers.Conversations.SearchProfiles(Pattern, Access.Communication.Session.Instance.AccountToken);

        IsSearching = false;
        MemberModels = result.Models;
        StateHasChanged();
    }



    /// <summary>
    /// Acción al crear.
    /// </summary>
    private async void Crear()
    {
        ActualState = State.Creating;
        StateHasChanged();

        var modelo = new ConversationModel()
        {
            ID = 0,
            Members = SelectedItems.Select(T => new MemberChatModel()
            {
                Rol = Types.Communication.Enumerations.MemberRoles.None,
                Profile = new()
                {
                    ID = T.Profile.ID
                }
            }).ToList(),
            Name = Name,
            Type = Types.Communication.Enumerations.ConversationsTypes.Group
        };

        modelo.Members.Add(new()
        {
            Profile = new()
            {
                ID = Access.Communication.Session.Instance.Profile.ID
            },
            Rol = Types.Communication.Enumerations.MemberRoles.Admin
        });

        var res = await Access.Communication.Controllers.Conversations.Create(modelo, LIN.Access.Communication.Session.Instance.Token);
        if (res.Response == Responses.Success)
        {
            ActualState = State.Success;
            StateHasChanged();
            OnSuccess();
            return;
        }

        ActualState = State.Failure;
        StateHasChanged();
    }





}
