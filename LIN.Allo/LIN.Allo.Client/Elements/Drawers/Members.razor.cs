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
    private List<Types.Auth.Abstracts.SessionModel<MemberChatModel>> MemberModels { get; set; } = new();



    /// <summary>
    /// Lista de controles de miembros.
    /// </summary>
    private List<Member> MemberControls { get; set; } = new();



    /// <summary>
    /// Control de un integrante.
    /// </summary>
    private Member MemberControl { set => MemberControls.Add(value); }



    /// <summary>
    /// Cache de miembros.
    /// </summary>
    private List<(int, List<Types.Auth.Abstracts.SessionModel<MemberChatModel>>)> Cache { get; set; } = new();



    /// <summary>
    /// Abrir el elemento.
    /// </summary>
    public async void Show()
    {
        await jsRuntime.InvokeVoidAsync("ShowDrawer", $"drawer-{UniqueId}", $"close-drawer-{UniqueId}");
        StateHasChanged();
    }


    public async Task Pre(int id)
    {

        var cache = Cache.Where(t => t.Item1 == id).FirstOrDefault();

        if (cache.Item2 == null)
        {
            Console.WriteLine("TO INTERNET");

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