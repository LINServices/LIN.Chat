namespace LIN.Allo.Client.Elements.Drawers;


public partial class Members
{


    /// <summary>
    /// Nombre del grupo.
    /// </summary>
    public string Name { get; set; } = string.Empty;



    /// <summary>
    /// Id único del elemento.
    /// </summary>
    private string UniqueId { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Lista de modelos de miembros.
    /// </summary>
    private List<Types.Auth.Abstracts.SessionModel<MemberChatModel>> MemberModels { get; set; } = [];



    /// <summary>
    /// Cache de miembros.
    /// </summary>
    private List<(int, List<Types.Auth.Abstracts.SessionModel<MemberChatModel>>)> Cache { get; set; } = [];



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



    /// <summary>
    /// Cargar los miembros.
    /// </summary>
    /// <param name="id">Id de la conversación.</param>
    public async Task LoadData(int id)
    {

        // Busca en el cache.
        var cache = Cache.Where(t => t.Item1 == id).FirstOrDefault();

        // Si no existe en el cache.
        if (cache.Item2 == null)
        {
            // Respuesta de la API.
            var result = await Access.Communication.Controllers.Conversations.MembersInfo(id, Access.Communication.Session.Instance.AccountToken);

            // Modelos a la UI.
            MemberModels = result.Models.OrderByDescending(t => t.Profile.Rol).ToList();
            Cache.Add(new(id, MemberModels));
        }
        else
        {
            MemberModels = cache.Item2;
        }

    }


}