using LIN.Access.Communication;
using LIN.Types.Cloud.Identity.Abstracts;

namespace LIN.Allo.Client.Elements.Drawers;


public partial class Members
{


    string Pattern;


    /// <summary>
    /// Name del grupo.
    /// </summary>
    public string Name { get; set; } = string.Empty;



    /// <summary>
    /// Id único del elemento.
    /// </summary>
    private string UniqueId { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Lista de modelos de miembros.
    /// </summary>
    private List<SessionModel<MemberChatModel>> MemberModels { get; set; } = [];



    /// <summary>
    /// Cache de miembros.
    /// </summary>
    private List<(int, List<SessionModel<MemberChatModel>>)> Cache { get; set; } = [];



    /// <summary>
    /// Lista de controles de miembros.
    /// </summary>
    private List<Member> MemberControls { get; set; } = [];



    /// <summary>
    /// Control de un integrante.
    /// </summary>
    private Member MemberControl { set => MemberControls.Add(value); }




    /// <summary>
    /// Abrir el elemento.
    /// </summary>
    public async void Show()
    {
        await jsRuntime.InvokeVoidAsync("ShowDrawer", $"drawer-{UniqueId}", $"close-drawer-{UniqueId}");
        StateHasChanged();
    }



    ConversationModel? ConversationContext { get; set; }

    /// <summary>
    /// Cargar los miembros.
    /// </summary>
    /// <param name="id">Id de la conversación.</param>
    public async Task LoadData(int id, bool force = false)
    {

        IsShowAdd = false;

        // Busca en el cache.
        var cache = Cache.Where(t => t.Item1 == id).FirstOrDefault();

        ConversationContext = new()
        {
            ID = id
        };

        // Si no existe en el cache.
        if (cache.Item2 == null || force)
        {
            // Respuesta de la API.
            var result = await Access.Communication.Controllers.Conversations.MembersInfo(id, Session.Instance.Token, Session.Instance.AccountToken);

            // Modelos a la UI.
            MemberModels = result.Models.OrderByDescending(t => t.Profile.Rol).ToList();

            Cache.RemoveAll(t => t.Item1 == id);
            Cache.Add(new(id, MemberModels));
        }
        else
        {
            MemberModels = cache.Item2;
        }

    }


    private List<SessionModel<ProfileModel>> SearchResult { get; set; } = new();

    /// <summary>
    /// Buscar elementos.
    /// </summary>
    private async void Search()
    {
        string pattern = Pattern;

        if (pattern.Trim() == "")
        {
            //AreSearch = false;
            //IsSearching = false;
            StateHasChanged();
            return;
        }

        //AreSearch = true;
        //IsSearching = true;
        StateHasChanged();
        var result = await Access.Communication.Controllers.Conversations.SearchProfiles(Pattern, Session.Instance.AccountToken);

        //IsSearching = false;
        SearchResult = result.Models;
        StateHasChanged();
    }



    /// <summary>
    /// Lista de controles de integrantes.
    /// </summary>
    private List<Profile> SearchControls { get; set; } = new();



    /// <summary>
    /// Control de integrante actual.
    /// </summary>
    private Profile SearchResultControl
    {
        set => SearchControls.Add(value);
    }



    bool IsShowAdd = false;
    void ShowAdd()
    {
        IsShowAdd = !IsShowAdd;
        StateHasChanged();
    }



    /// <summary>
    /// Items seleccionados.
    /// </summary>
    private List<SessionModel<ProfileModel>> NewMembers { get; set; } = new();



    /// <summary>
    /// Al seleccionar un elemento.
    /// </summary>
    /// <param name="model">Modelo seleccionado.</param>
    private void OnSelect(SessionModel<ProfileModel> model)
    {
        // Si existe.
        var have = NewMembers.Where(T => T.Account.Id == model.Account.Id).Any();

        if (have)
        {
            NewMembers.RemoveAll(T => T.Account.Id == model.Account.Id);
            StateHasChanged();
            return;
        }

        NewMembers.Add(model);
        StateHasChanged();
    }


    async void Insert()
    {
        if (ConversationContext == null)
        {
            return;
        }

        List<Task> tasks = [];

        var token = Session.Instance.Token;
        foreach (var x in NewMembers)
        {
            tasks.Add(Access.Communication.Controllers.Conversations.Insert(ConversationContext.ID, x.Profile.ID, token));
        }

        await Task.WhenAll(tasks);

        NewMembers = [];
        IsShowAdd = false;
        await LoadData(ConversationContext.ID, true);

        StateHasChanged();

    }

}